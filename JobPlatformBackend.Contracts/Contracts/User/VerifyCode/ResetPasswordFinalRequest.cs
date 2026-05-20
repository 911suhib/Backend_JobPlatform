using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPlatformBackend.Contracts.Contracts.User.VerifyCode
{
	public record ResetPasswordFinalRequest(
		string Email,
		string Code,
		string NewPassword
	);
}
