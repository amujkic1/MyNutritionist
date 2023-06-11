namespace MyNutritionist.Utilities
{
    public class ProtectionProxy : ISubject
    {
            private readonly RealSubject _realSubject;

            public ProtectionProxy()
            {
                _realSubject = new RealSubject();
            }

            public bool Login(string username, string password)
            {
                if (ValidateRequest(username, password))
                {
                    return _realSubject.Login(username, password);
                }

                
                return false;
            }

            private bool ValidateRequest(string username, string password)
            {
                if(username == null || password == null){
                return false;
            }

            return true;
            }
    }
}
