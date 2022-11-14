using System.Collections.Generic;
using System.IO;

namespace Portals_of_Madness
{
    public struct Response
    {
        public bool Verdict { get; }
        public string Message { get; }

        public Response(bool v, string m)
        {
            Verdict = v;
            Message = m;
        }
    }

    public class Tests
    {
        private Controller Cont { get; set; }
        private int Counter { get; set; }

        public Tests(Controller c)
        {
            Cont = c;
            Counter = 0;
        }

        private Response ComparisonShouldSucceed<T, S>(T First, S Second)
        {
            if (First.Equals(Second))
            {
                return new Response(First.Equals(Second), "");
            }
            return new Response(First.Equals(Second), $"{First} != {Second}");
        }

        private Response ComparisonShouldFail<T, S>(T First, S Second)
        {
            if (!First.Equals(Second))
            {
                return new Response(!First.Equals(Second), "");
            }
            return new Response(!First.Equals(Second), $"{First} == {Second}");
        }

        private string Verdict(bool result, string testname = "Default Test Name", string info = "")
        {
            Counter++;
            if (result)
            {
                return $"Test {Counter}: [OK]\n{testname}\n";
            }
            return $"Test {Counter}: [Failed]\n{testname}\n{info}\n";
        }

        public void RunTests()
        {
            List <string> lines = new List<string>();
            StreamWriter file = new StreamWriter("Test Results.txt");

            Character TestSubject1 = new Character(n: "Test Subject 1",
                ab2: new Ability("Resurrect", abT: "resurrect"),
                ab3: new Ability("Heal", 5, fAD: 0, mAD: 2, tT: "ally", abT: "heal"));
            Character TestSubject2 = new Character(n: "Test Subject 2");
            Character TestSubject3 = new Character(n: "Test Subject 3", pAr: 100, pArM: 1, mAr: 100, mArM: 1);

            Response Test;
            string TestName;
            Counter = 0;

            //Test 1

            Test = ComparisonShouldFail(TestSubject1, TestSubject2);
            TestName = $"Comparing {TestSubject1.Name} {TestSubject2.Name}";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 2
            TestSubject1.CastAbility(TestSubject1.Abilities[0], TestSubject2);

            Test = ComparisonShouldSucceed(80.0, TestSubject2.CurrentHealth);
            TestName = $"Damaging {TestSubject2.Name} and then comparing remaining health to what it should be (80)";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 3

            TestSubject1.SetLevel(40);

            Test = ComparisonShouldSucceed(25, TestSubject1.Level);
            TestName = $"Setting {TestSubject1.Name}'s level over the level cap and then comparing it to the level cap (25)";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 4

            Test = ComparisonShouldSucceed(true, TestSubject1.MaxHealth > 100);
            TestName = $"Checking if {TestSubject1.Name}'s max health has increased";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 5

            double tmpHealthMissing = TestSubject2.MaxHealth - TestSubject2.CurrentHealth;
            double tmpCurrentHealth = TestSubject2.CurrentHealth;

            TestSubject1.CastAbility(TestSubject1.Abilities[0], TestSubject2);

            Test = ComparisonShouldSucceed(true, tmpCurrentHealth - TestSubject2.CurrentHealth > tmpHealthMissing);
            TestName = $"Damaging {TestSubject2.Name} again and checking if the health decreased more than last time";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 6

            Test = ComparisonShouldSucceed(1.0, TestSubject2.CurrentHealth);
            TestName = $"Checking if {TestSubject2.Name} has 1 health";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 7

            Test = ComparisonShouldSucceed(true, TestSubject2.Alive);
            TestName = $"Checking if {TestSubject2.Name} is still alive due to healthgate";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 8

            TestSubject1.CastAbility(TestSubject1.Abilities[0], TestSubject2);

            Test = ComparisonShouldSucceed(0.0, TestSubject2.CurrentHealth);
            TestName = $"Checking if {TestSubject2.Name} has 0 health";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 9

            Test = ComparisonShouldSucceed(false, TestSubject2.Alive);
            TestName = $"Checking if {TestSubject2.Name} is dead";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 10

            TestSubject1.CastAbility(TestSubject1.Abilities[1], TestSubject2);

            Test = ComparisonShouldSucceed(true, TestSubject2.Alive);
            TestName = $"Resurrecting {TestSubject2.Name}";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 11

            Test = ComparisonShouldSucceed(20.0, TestSubject2.CurrentHealth);
            TestName = $"Checking if {TestSubject2.Name} has 20 health after resurrection";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 12

            TestSubject1.SetLevel(1);

            Test = ComparisonShouldSucceed(10, TestSubject1.CurrentResource);
            TestName = $"Checking if {TestSubject1.Name} has the correct amount of mana";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 13

            TestSubject1.CastAbility(TestSubject1.Abilities[2], TestSubject2);

            Test = ComparisonShouldSucceed(40.0, TestSubject2.CurrentHealth);
            TestName = $"Checking if {TestSubject2.Name} has 40 health after healing";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 14

            Test = ComparisonShouldSucceed(5, TestSubject1.CurrentResource);
            TestName = $"Checking if {TestSubject1.Name} has the correct amount of mana after using some";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            //Test 15

            Test = ComparisonShouldFail(true, TestSubject3.CurrentHealth < TestSubject3.MaxHealth);
            TestName = $"Testing armor";

            lines.Add(Verdict(Test.Verdict, TestName, Test.Message));

            // End of testing

            foreach (string line in lines)
            {
                file.WriteLine(line);
            }

            file.Close();
        }
    }
}
