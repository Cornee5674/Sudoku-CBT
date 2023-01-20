using System;
namespace SudokuCBT
{
	public class DomainsAndConstraints
	{
		public List<int> domain;
		public List<int> constraints;

		public DomainsAndConstraints()
		{
			// Domain of spot in Sudoku
			domain = new List<int>();
			// The list of constraints are the number/numbers that CAN be in a specific spot
			constraints = new List<int>();


			// Initialize the domain and constraint list to 1-9
			for (int i = 1; i <= 9; i++)
			{
				domain.Add(i);
				constraints.Add(i);
			}
		}

		// Debug function for constraint list
		public void PrintConstraintList()
		{
			string x = "";
			for (int i = 0; i < constraints.Count; i++)
			{
				x += constraints[i] + "|";
			}
			Console.WriteLine(x);
		}

		// Debug function for domain list
		public void PrintDomainList()
		{
			string x = "";
			for (int i = 0; i < domain.Count; i++)
			{
				x += domain[i] + "|";
			}
			Console.WriteLine(x);
		}

		// Function to call if a specific spot has an assigned number. We then change the constraint list to only this number.
		public void AddConstraintsIfGivenNumber(int nr)
		{
            constraints = new List<int>
            {
                nr
            };
        }

		public void RemoveConstraint(int nr)
		{
			// Filter the given number out of the constraint list
			constraints.RemoveAll(item => item == nr);
		}

		public void IntersectConstraintWithDomain()
		{
			// Intersect the domain list with the constraint list
			domain = domain.Intersect(constraints).ToList();
		}
	}
}

