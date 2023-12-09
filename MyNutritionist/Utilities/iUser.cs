namespace MyNutritionist.Utilities
{
    /*
     * iUser interfejs definiše osnovne metode za inicijalizaciju informacija o korisniku.
     */
    public interface iUser
    {
        /*
         * Metoda za inicijalizaciju mjesta prebivališta korisnika.
         *
         * @param city: Naziv grada.
         */
        void InitializeCity(string city);

        /*
         * Metoda za inicijalizaciju težine korisnika.
         *
         * @param weight: Težina korisnika u kilogramima.
         */
        void InitializeWeight(double weight);

        /*
         * Metoda za inicijalizaciju visine korisnika.
         *
         * @param height: Visina korisnika u centimetrima.
         */
        void InitializeHeight(double height);
    }
}
