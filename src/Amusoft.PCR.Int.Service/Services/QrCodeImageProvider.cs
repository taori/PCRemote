using System.Text;
using Amusoft.PCR.AM.Service.Interfaces;
using QRCoder;

namespace Amusoft.PCR.Int.Service.Services;

public class QrCodeImageProvider : IQrCodeImageProvider
{
	public byte[] GetQrCode(string base64)
	{
		var content = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
		var generator = new QRCodeGenerator();
		var qrCode = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
		var png = new PngByteQRCode(qrCode);

		return png.GetGraphic(10);
	}
}