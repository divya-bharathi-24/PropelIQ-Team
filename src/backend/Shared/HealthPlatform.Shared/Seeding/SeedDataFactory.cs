using HealthPlatform.Shared.Data.Entities;

namespace HealthPlatform.Shared.Seeding;

/// <summary>
/// Factory for generating realistic test data.
/// All data is deterministic based on index for reproducibility.
/// </summary>
public static class SeedDataFactory
{
    private static readonly string[] FirstNames =
    {
        "James", "Mary", "Robert", "Patricia", "John", "Jennifer",
        "Michael", "Linda", "David", "Elizabeth", "William", "Barbara",
        "Richard", "Susan", "Joseph", "Jessica", "Thomas", "Sarah",
        "Christopher", "Karen", "Daniel", "Lisa", "Matthew", "Nancy",
        "Anthony", "Betty", "Mark", "Margaret", "Donald", "Sandra",
        "Steven", "Ashley", "Paul", "Kimberly", "Andrew", "Emily",
        "Joshua", "Donna", "Kenneth", "Michelle", "Kevin", "Dorothy",
        "Brian", "Carol", "George", "Amanda", "Timothy", "Melissa",
        "Ronald", "Deborah"
    };

    private static readonly string[] LastNames =
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia",
        "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez",
        "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore",
        "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
        "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
        "Walker", "Young", "Allen", "King", "Wright", "Scott",
        "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams",
        "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
        "Carter", "Roberts"
    };

    public static List<Patient> CreatePatients(int count)
    {
        var patients = new List<Patient>(count);
        for (var i = 0; i < count; i++)
        {
            patients.Add(new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = FirstNames[i % FirstNames.Length],
                LastName = LastNames[i % LastNames.Length],
                DateOfBirth = DateOnly.FromDateTime(new DateTime(1950 + (i % 50), (i % 12) + 1, (i % 28) + 1)),
                Gender = i % 2 == 0 ? "Male" : "Female",
                Email = $"patient{i + 1}@example.com",
                Phone = $"555-{(1000 + i):D4}",
                IsActive = true,
            });
        }
        return patients;
    }

    public static List<PatientContact> CreateContacts(Guid patientId)
    {
        return new List<PatientContact>
        {
            new()
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                ContactName = "Emergency Contact",
                Relationship = "Spouse",
                Phone = "555-0000",
                IsPrimary = true,
            }
        };
    }

    public static List<InsuranceDetail> CreateInsurance(Guid patientId)
    {
        return new List<InsuranceDetail>
        {
            new()
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                ProviderName = "Blue Cross Blue Shield",
                PolicyNumber = $"BCBS-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
                EffectiveDate = new DateOnly(2024, 1, 1),
                IsPrimary = true,
            }
        };
    }
}
