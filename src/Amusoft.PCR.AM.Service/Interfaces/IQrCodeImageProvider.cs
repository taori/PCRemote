namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IQrCodeImageProvider
{
	byte[] GetQrCode(string base64);
}