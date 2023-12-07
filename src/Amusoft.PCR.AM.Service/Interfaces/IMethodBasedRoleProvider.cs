using System.Reflection;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IMethodBasedRoleProvider
{
	MethodInfo[] GetMethods();
}