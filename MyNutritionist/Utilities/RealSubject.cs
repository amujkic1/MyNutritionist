using MyNutritionist.Data;
using MyNutritionist.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyNutritionist.Utilities
{
    /*
     * RealSubject klasa predstavlja stvarni subjekat (RealSubject) u Protection Proxy design patternu
     * @implements ISubject interfejs.
     */
    public class RealSubject : ISubject
    {
        private readonly ApplicationDbContext _dbContext;

        /*
         * Konstruktor RealSubject klase bez parametara.
         */
        public RealSubject()
        {
        }

        /*
         * Konstruktor RealSubject klase sa parametrom za pristup bazi podataka.
         *
         * @param dbContext: Kontekst baze podataka.
         */
        public RealSubject(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
         * Metoda za prijavu korisnika.
         *
         * @param username: Korisničko ime.
         * @param password: Šifra korisnika.
         *
         * @return: True ako je prijava uspješna, inače false.
         */
        public bool Login(string username, string password)
        {
            try
            {
                // Pronađi korisnika u bazi podataka prema korisničkom imenu
                var user = _dbContext.Users.FirstOrDefault(u => u.UserName == username);

                // Provjeri da li korisnik postoji i da li se šifre podudaraju
                if (user != null && VerifyPassword(password, user.PasswordHash))
                {
                    return true;
                }

                // Neuspjela prijava ako korisnik ne postoji ili šifre nisu ispravne
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*
         * Privatna metoda za provjeru podudarnosti unesene šifre i spremljene hash-ovane šifre 
         * u bazi podataka.
         *
         * @param password: Unesena šifra.
         * @param hashedPassword: Spremljena hash šifra iz baze podataka.
         *
         * @return: True ako su lozinke podudarne, inače false.
         */
        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                // Računanje "hash" vrijednosti unesene šifre
                byte[] passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Konvertovanje "hash" lozinke iz niza bajtova u string
                string hashedPasswordToCompare = BitConverter.ToString(passwordHash).Replace("-", "");

                // Poređenje hash-ovanih šifri
                return string.Equals(hashedPassword, hashedPasswordToCompare, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
