namespace MyNutritionist.Utilities
{
    /// Interface for subjects that support login functionality.
    public interface ISubject
    {
        /// Attempts to authenticate a user based on the provided username and password.
        /// "username" the username of the user attempting to log in.
        /// "password" the password of the user attempting to log in.
        /// returns ztrue if the login is successful; otherwise, false.
        bool Login(string username, string password);
    }
}
