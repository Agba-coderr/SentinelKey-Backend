namespace SentinelKey.Application.Common.Abstractions.Security;

public interface IOtpSecretProtector
{
    string GenerateSecret();
    string Protect(string secret);
    string Unprotect(string protectedSecret);
}
