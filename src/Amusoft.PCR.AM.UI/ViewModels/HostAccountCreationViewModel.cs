using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountCreationViewModel : PageViewModel, INavigationCallbacks
{
	private readonly ILogger<HostAccountCreationViewModel> _logger;
	private readonly IBearerTokenRepository _bearerTokenRepository;
	private readonly Endpoint _endpoint;
	private readonly IHostCredentials _hostCredentials;
	private readonly IToast _toast;
	private readonly IIpcIntegrationService _ipcIntegrationService;
	private readonly IIdentityManagerFactory _identityManagerFactory;
	private readonly IEndpointRepository _endpointRepository;

	[ObservableProperty]
	private string _hostAccountDisplayName;

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(HostAccountCreationViewModel), nameof(ValidateEmail))]
	private string _email;

	[ObservableProperty]
	private string? _emailError;

	[ObservableProperty]
	[NotifyDataErrorInfo]
	[CustomValidation(typeof(HostAccountCreationViewModel), nameof(ValidatePassword))]
	private string _password;

	[ObservableProperty]
	private string? _passwordError;

	private readonly HashSet<string> EndPointEmails = new(StringComparer.OrdinalIgnoreCase);

	public HostAccountCreationViewModel(
		ILogger<HostAccountCreationViewModel> logger,
		IBearerTokenRepository bearerTokenRepository,
		ITypedNavigator navigator,
		Endpoint endpoint,
		IHostCredentials hostCredentials,
		IToast toast,
		IIpcIntegrationService ipcIntegrationService,
		IIdentityManagerFactory identityManagerFactory,
		IEndpointRepository endpointRepository) : base(navigator)
	{
		_logger = logger;
		_bearerTokenRepository = bearerTokenRepository;
		_endpoint = endpoint;
		_hostCredentials = hostCredentials;
		_toast = toast;
		_ipcIntegrationService = ipcIntegrationService;
		_identityManagerFactory = identityManagerFactory;
		_endpointRepository = endpointRepository;

		this.ErrorsChanged += OnErrorsChanged;

		void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Email))
				EmailError = GetErrors(nameof(Email)).FirstOrDefault(d => d.ErrorMessage != null)?.ErrorMessage;
			if (e.PropertyName == nameof(Password))
				PasswordError = GetErrors(nameof(Password)).FirstOrDefault(d => d.ErrorMessage != null)?.ErrorMessage;
		}
	}

	public static ValidationResult ValidateEmail(string email, ValidationContext validationContext)
	{
		if (string.IsNullOrEmpty(email))
			return new ValidationResult(Translations.HostAccountCreation_EmailCannotBeEmpty);

		if (validationContext.ObjectInstance is HostAccountCreationViewModel viewModel)
		{
			if (viewModel.EndPointEmails.Contains(email))
			{
				return new ValidationResult(Translations.HostAccountCreation_EmailAlreadyInUse);
			}
		}

		return ValidationResult.Success;
	}

	public static ValidationResult ValidatePassword(string password, ValidationContext validationContext)
	{
		if (string.IsNullOrEmpty(password) || password.Length < 3)
			return new ValidationResult(Translations.HostAccountCreation_PasswordLengthRequired);
		return ValidationResult.Success;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccountCreation_Title;
	}


	[RelayCommand]
	private void UpdateEmail()
	{
		ValidateProperty(Email, nameof(Email));
	}

	[RelayCommand]
	private async Task CompleteCreation()
	{
		ValidateAllProperties();
		if (HasErrors)
			return;
		
		var identityManager = _identityManagerFactory.Create(_hostCredentials.Address, _hostCredentials.Protocol);
		if (!await TryRegisterNewUserAsync(identityManager))
			return;
		var token = await identityManager.LoginAsync(Email, Password, CancellationToken.None);
		if (token is null)
			return;
		if (!await TrySaveEndpointAsync(token))
			return;

		await _toast.Make(Translations.Generic_ActionSucceeded).SetPosition(Position.Top).Show();
		await Navigator.PopAsync();
	}

	private async Task<bool> TrySaveEndpointAsync(SignInResponse token)
	{
		try
		{
			var account = await _endpointRepository.CreateEndpointAccountAsync(_endpoint.Id, Email, CancellationToken.None);
			var bearerToken = new BearerToken()
			{
				AccessToken = token.AccessToken,
				RefreshToken = token.RefreshToken,
				Expires = token.ValidUntil,
				IssuedAt = DateTimeOffset.Now,
				EndpointAccountId = account.Id,
			};

			return await _bearerTokenRepository.AddTokenAsync(bearerToken, CancellationToken.None);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to save endpoint");
			return false;
		}
	}

	private async Task<bool> TryRegisterNewUserAsync(IIdentityManager identityManager)
	{
		try
		{
			await identityManager.RegisterAsync(Email, Password, CancellationToken.None);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to register user");
			return false;
		}
	}

	public Task OnNavigatedToAsync()
	{
		HostAccountDisplayName = _hostCredentials.Title;
		return UpdateEndpointEmailsAsync();
	}

	private async Task UpdateEndpointEmailsAsync()
	{
		var accounts = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address, CancellationToken.None)
			.ConfigureAwait(false);
		EndPointEmails.Clear();
		foreach (var account in accounts)
		{
			EndPointEmails.Add(account.Email);
		}
	}
}