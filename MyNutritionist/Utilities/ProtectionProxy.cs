namespace MyNutritionist.Utilities
{
    /*
     * ProtectionProxy klasa predstavlja zamjenski objekat (proxy) koji štiti stvarni subjekat 
     * (RealSubject) od neovlaštenih pristupa.
     * 
     * @implements ISubject interfejs.
     */
    public class ProtectionProxy : ISubject
    {
        private readonly RealSubject _realSubject;

        /*
         * Konstruktor ProtectionProxy klase.
         * Inicijalizira stvarni subjekat (RealSubject).
         */
        public ProtectionProxy()
        {
            _realSubject = new RealSubject();
        }

        /*
         * Metoda za prijavu korisnika sa provjerom permisija.
         *
         * @param username: Korisničko ime.
         * @param password: Šifra korisnika.
         *
         * @return: True ako je prijava uspješna, inače false.
         */
        public bool Login(string username, string password)
        {
            if (ValidateRequest(username, password))
            {
                return _realSubject.Login(username, password);
            }

            return false;
        }

        /*
         * Privatna metoda za validaciju korisničkih podataka prije prijave.
         *
         * @param username: Korisničko ime.
         * @param password: Šifra korisnika.
         *
         * @return: True ako su podaci validni, inače false.
         */
        private bool ValidateRequest(string username, string password)
        {
            /*
             * Provjera da li su korisničko ime i šifra postavljeni.
             */
            if (username == null || password == null)
            {
                return false;
            }

           
            return true;
        }
    }
}
