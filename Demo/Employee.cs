using System;
using System.IO;
using Stringier.Patterns;

namespace Demo {
	public sealed class Employee {
		private String name;

		public String Name {
			get => name;
			set {
				name = value;
				Store();
			}
		}

		private Sex sex;

		public Sex Sex {
			get => sex;
			set {
				sex = value;
				Store();
			}
		}

		private DateTime birthdate;

		public DateTime Birthdate {
			get => birthdate;
			set {
				birthdate = value;
				Store();
			}
		}

		private String title;

		public String Title {
			get => title;
			set {
				title = value;
				Store();
			}
		}

		public Employee(String name, Sex sex, DateTime birthdate, String title) {
			this.name = name;
			this.sex = sex;
			this.birthdate = birthdate;
			this.title = title;
		}

		public void Store() {
			using StreamWriter file = new StreamWriter($"{Name}.record");
			file.WriteLine($"Sex: {Sex}");
			file.WriteLine($"Birthdate: {Birthdate}");
			file.WriteLine($"Title: {Title}");
		}

		public static Employee Load(String name) {
			using StreamReader file = new StreamReader($"{name}.record");
			String line;
			Sex sex = default;
			DateTime birthdate = default;
			String title = default;
			while ((line = file.ReadLine()) != null) {
				Source source = new Source(line);
				if ("Sex: ".Consume(ref source)) {
					sex = Enum.Parse<Sex>(source.ToString());
				} else if ("Birthdate: ".Consume(ref source)) {
					birthdate = DateTime.Parse(source.ToString());
				} else if ("Title: ".Consume(ref source)) {
					title = source.ToString();
				} else {
					return null;
				}
			}
			return new Employee(name, sex, birthdate, title);
		}
	}
}
