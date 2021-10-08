using System;

namespace feleves_feladat
{
    class Section
    {
        int id;


        public Section(int id) { this.id = id; }

        public int GetId() { return id; }
    }

    class Field
    {
        int numberOfSections;
        Section[] sections;


        public Field(int numberOfSections) 
        {
            this.numberOfSections = numberOfSections;
            this.sections = new Section[numberOfSections];
        }

        public int GetNumberOfSections() { return numberOfSections; }

        public Section[] GetSections() { return sections; }
    }

    class Applicant
    {
        int id;
        int first;
        int last;
        int price;
        double unitPrice;
        bool winner;

        public Applicant(int id, int first, int last, int price)
        {
            this.id = id;
            this.first = first;
            this.last = last;
            this.price = price;
            winner = false;
            SetUnitPrice();
        }

        public bool IsWinner() { return winner; }

        public void SetWinner() { winner = true; }

        public void SetLoser() { winner = false; }

        // Megvizsgálja, hogy az adott pályázó parcellái között és sorbarendezés után, a sorban előtte álló pályázók parcallái között
        // van-e átfedés.
        public bool IsIntersect(Applicant[] applicants, int current)
        {
            for (int i = 0; i <= current; i++)
                if (!(applicants[i].first > this.last || this.first > applicants[i].last))
                    return true;

            return false;
        }

        public int GetId() { return id; }

        public int GetFirst() { return first; }

        public int GetLast() { return last; }

        public int GetPrice() { return price; }

        public double GetUnitPrice() { return unitPrice; }

        public void SetUnitPrice() { unitPrice = price / (last - first + 1); }
    }

    class Applications
    {
        int numberOfApplicants;
        Applicant[] applicants;
        double maxIncome;

        public Applications(int numberOfApplicants)
        {
            this.numberOfApplicants = numberOfApplicants;
            this.applicants = new Applicant[numberOfApplicants];
            this.maxIncome = 0;
        }

        public void ResetWinners()
        {
            for (int i = 0; i < applicants.Length; i++)
                applicants[i].SetLoser();
        }

        // Kiírja a nyertes pályázókat.
        public void PrintWinners()
        {
            Console.WriteLine(maxIncome);
            for (int i = 0; i < applicants.Length; i++)
                if (applicants[i].IsWinner())
                    Console.Write(applicants[i].GetId() + " ");
        }

        // Eldönti a nyertes pályázókat. Egységár szerinti csökkenő sorbarendezés után elindul az első pályázótól, megjelöli őt nyertesként
        // és az általa kínált teljes árat a maxIncome nevű változóba teszi. Utána tovább megy a sorrendben a következő pályázóra, majd
        // egyesével megvizsgálja, hogy az új pályázó parcellái és a sorrendben előtte lévő pályázók parcellái között van-e átfedés.
        // Ha nincs átfedés, akkor az új pályázót is megjelöli nyertésként és a maxIncome-hoz hozzáadja az új pályázó által kínált teljes árat.
        // Ha van átfedés és az új pályázó által kínált ár nagyobb, mint az eddigi nyertes pályázók által kínált árak összegezve, akkor a 
        // az eddig nyertes pályázókat megjelöli vesztesként, és az új pályázó lesz a nyertes, illetve a maxIncome is az általa kínált
        // teljes ár lesz. Majd megy tovább egészen, amíg vannak meg nem vizsgált pályázók.
        public void FindWinners()
        {
            SortApplicants();
            maxIncome = applicants[0].GetPrice();
            applicants[0].SetWinner();
            for (int i = 1; i < applicants.Length; i++)
            {
                if (!applicants[i].IsIntersect(applicants, i-1))
                {   
                    maxIncome += applicants[i].GetPrice();
                    applicants[i].SetWinner();
                }
                else if (applicants[i].IsIntersect(applicants, i-1) && maxIncome < applicants[i].GetPrice())
                {
                    ResetWinners();
                    maxIncome = applicants[i].GetPrice();
                    applicants[i].SetWinner();
                }
            }
        }

        // Csökkenő sorrendbe rendeti a pályázókat egységár alapján.
        public void SortApplicants()
        {
            Applicant temp;
            for (int i = 0; i < applicants.Length - 1; i++)
            {
                for (int j = 0; j < applicants.Length - 1; j++)
                    if (applicants[j].GetUnitPrice() < applicants[j + 1].GetUnitPrice())
                    {
                        temp = applicants[j + 1];
                        applicants[j + 1] = applicants[j];
                        applicants[j] = temp;
                    }
            }
        }

        public int GetNumberOfApplicants() { return numberOfApplicants; }

        public Applicant[] GetApplicants() { return applicants; }

        // Kiszámolja hányan licitálnak a  megadott parcellára.
        public int GetNumberOfApplicantsForSection(Section section)
        {
            int result = 0;
            for (int i = 0; i < applicants.Length; i++)
                for (int j = applicants[i].GetFirst(); j <= applicants[i].GetLast(); j++)
                    if (j == section.GetId())
                        result++;

            return result;
        }

        // Kigyűjti kik licitálnak az ID-val megadott parcellára.
        public Applicant[] GetApplicantsForSection(Section section)
        {
            Applicant[] result = new Applicant[GetNumberOfApplicantsForSection(section)];
            int k = 0;
            for (int i = 0; i < applicants.Length; i++)
                for (int j = applicants[i].GetFirst(); j <= applicants[i].GetLast(); j++)
                    if (j == section.GetId())
                        result[k++] = applicants[i];

            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LICIT.BE");
            // Első sor, legalapabb adatok bekérése. (0 - pályázatok száma, 1 - parcellák száma)
            string firstLine = Console.ReadLine();
            while (int.Parse(firstLine.Split(" ")[0]) < 1 || int.Parse(firstLine.Split(" ")[0]) > 100 ||
                   int.Parse(firstLine.Split(" ")[1]) < 1 || int.Parse(firstLine.Split(" ")[1]) > 100)
                firstLine = Console.ReadLine();

            Applications applications = new Applications(int.Parse(firstLine.Split(" ")[0]));
            Field field = new Field(int.Parse(firstLine.Split(" ")[1]));
            string[] input = new string[applications.GetApplicants().Length];

            for (int i = 0; i < applications.GetApplicants().Length; i++)
                input[i] = Console.ReadLine();

            // Pályazók betöltése. (ID-k számozása 1-től indul)
            for (int i = 0; i < applications.GetNumberOfApplicants(); i++)
                applications.GetApplicants()[i] = new Applicant(i+1, int.Parse(input[i].Split(" ")[0]), 
                                                                     int.Parse(input[i].Split(" ")[1]),
                                                                     int.Parse(input[i].Split(" ")[2]));

            // Parcellák betöltése. (ID-k számozása 1-től indul)
            for (int i = 0; i < field.GetNumberOfSections(); i++)
                field.GetSections()[i] = new Section(i+1);

            applications.FindWinners();
            Console.WriteLine("\nLICIT.KI");
            applications.PrintWinners();
        }
    }
}
