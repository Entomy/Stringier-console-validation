using System;
using System.IO;
using System.Text.RegularExpressions;
using Consolator.UI;
using Consolator.UI.Theming;
using Console = Consolator.Console;
using Stringier;
using Stringier.Patterns;
using static Stringier.Patterns.Pattern;

namespace Demo {
	public static class Program {
		private static readonly KeyChoiceSet Menu = new KeyChoiceSet(" Enter Choice: ",
			new KeyChoice(ConsoleKey.A, "Add Employee", AddEmployee),
			new KeyChoice(ConsoleKey.L, "List Employees", ListEmployees),
			new KeyChoice(ConsoleKey.R, "Remove Employee", RemoveEmployee),
			new KeyChoice(ConsoleKey.U, "Update Employee", UpdateEmployee),
			new BackKeyChoice(ConsoleKey.Q, "Quit", () => Environment.Exit(0)));

		public static void Main() {
			Theme.DefaultDark.Apply();

			while (true) {
				Console.WriteChoices(Menu);
				Console.ReadChoice(Menu);
			}
		}

		private static readonly Pattern sexPattern = "Male".With(Compare.CaseInsensitive).Or("Female".With(Compare.CaseInsensitive));

		private static readonly Pattern birthdatePattern = new Regex("^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}").AsPattern();

		#region Add Employee
		private static void AddEmployee() {
			String name = Console.ReadLine(" Name: ", ConsoleColor.Yellow, ConsoleColor.White).Clean();
			String title = Console.ReadLine(" Title: ", ConsoleColor.Yellow, ConsoleColor.White).Clean();
			SexPrompt:
			String sex = Console.ReadLine(" Sex: ", ConsoleColor.Yellow, ConsoleColor.White).Clean();
			if (!sexPattern.Consume(sex)) {
				Console.WriteLine($"ERROR: {sex} is not a valid sex, expected Male or Female", ConsoleColor.Red);
				goto SexPrompt;
			}
			BirthdatePrompt:
			String birthdate = Console.ReadLine(" Birthdate: ", ConsoleColor.Yellow, ConsoleColor.White).Clean();
			if (!birthdatePattern.Consume(birthdate)) {
				Console.WriteLine($"ERROR: {birthdate} is not a valid birthdate, expected an ISO date format", ConsoleColor.Red);
				goto BirthdatePrompt;
			}
			Console.WriteLine(" Creating record...", ConsoleColor.DarkCyan);
			new Employee(name, Enum.Parse<Sex>(sex), DateTime.Parse(birthdate), title).Store();
			Console.WriteLine(" Record created successfully", ConsoleColor.DarkCyan);
		}
		#endregion

		#region List Employees
		private static void ListEmployees() {
			Employee employee;
			Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,10}|{3,20}|", "Name".Pad(20), "Title".Pad(20), "Sex".Pad(10), "Birthdate".Pad(20)), ConsoleColor.Blue);
			foreach (String fileName in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.record")) {
				employee = Employee.Load(Path.GetFileNameWithoutExtension(fileName));
				Console.WriteLine(String.Format("|{0,20}|{1,20}|{2,10}|{3,20}|", employee.Name, employee.Title, employee.Sex, employee.Birthdate.ToShortDateString()));
			}
		}
		#endregion

		#region Remove Employee
		private static void RemoveEmployee() {
			NamePrompt:
			String name = Console.ReadLine(" Name: ", ConsoleColor.Yellow, ConsoleColor.White);
			if (File.Exists($"{name}.record")) {
				File.Delete($"{name}.record");
			} else {
				Console.WriteLine($"ERROR: \"{name}\" wasn't found in the registry", ConsoleColor.Red);
				goto NamePrompt;
			}
		}
		#endregion

		#region Update Employee
		private static Boolean updateLoop = true;

		private static Employee employee;

		private static readonly KeyChoiceSet UpdateMenu = new KeyChoiceSet(" Enter Choice: ",
			new KeyChoice(ConsoleKey.B, "Birthdate", UpdateBirthdate),
			new KeyChoice(ConsoleKey.S, "Sex", UpdateSex),
			new KeyChoice(ConsoleKey.T, "Title", UpdateTitle),
			new BackKeyChoice(ConsoleKey.D, "Done", () => {
				updateLoop = false;
				employee = null;
			}));

		private static void UpdateEmployee() {
		NamePrompt:
			String name = Console.ReadLine(" Name: ", ConsoleColor.Yellow, ConsoleColor.White);
			if (File.Exists($"{name}.record")) {
				employee = Employee.Load(name);
			} else {
				Console.WriteLine($"ERROR: \"{name}\" wasn't found in the registry", ConsoleColor.Red);
				goto NamePrompt;
			}
			updateLoop = true;
			while (updateLoop) {
				Console.WriteChoices(UpdateMenu);
				Console.ReadChoice(UpdateMenu);
			}
		}

		private static void UpdateBirthdate() {
			BirthdatePrompt:
			String birthdate = Console.ReadLine(" Birthdate: ", ConsoleColor.Yellow, ConsoleColor.White);
			if (birthdatePattern.Consume(birthdate)) {
				employee.Birthdate = DateTime.Parse(birthdate);
			} else {
				Console.WriteLine($"ERROR: {birthdate} is not a valid birthdate, expected an ISO date format", ConsoleColor.Red);
				goto BirthdatePrompt;
			}
		}

		private static void UpdateSex() {
			SexPrompt:
			String sex = Console.ReadLine(" Sex: ", ConsoleColor.Yellow, ConsoleColor.White);
			if (sexPattern.Consume(sex)) {
				employee.Sex = Enum.Parse<Sex>(sex);
			} else {
				Console.WriteLine($"ERROR: {sex} is not a valid sex, expected Male or Female", ConsoleColor.Red);
				goto SexPrompt;
			}
		}

		private static void UpdateTitle() {
			String title = Console.ReadLine(" Title: ", ConsoleColor.Yellow, ConsoleColor.White);
			employee.Title = title;
		}
		#endregion
	}
}
