using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp.DataBase.Τhesis
{
    public class EmployeeDataEntity
    {
        [Key]
        public int EmployeeID { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Code { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Descr { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Gender { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Address { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string ContactNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Position { get; set; }

        [Column(TypeName = "float")]
        public float? TotalFlightHours { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? HireDate { get; set; }

        [Column(TypeName = "int")]
        public int? Seniority { get; set; }

        [Column(TypeName = "int")]
        public int? Language { get; set; }

        [Column(TypeName = "bool")]
        public bool IsDeleted { get; set; }

        public int? BaseAirportId { get; set; }
        public int? CertificationID { get; set; }

        [Column(TypeName = "int")]
        public int? LowerBound { get; set; }

        [Column(TypeName = "int")]
        public int? UpperBound { get; set; }

        [ForeignKey("BaseAirportId")]
        public virtual AirportsDataEntity Airports { get; set; }

        [ForeignKey("CertificationID")]
        public virtual CertificationsDataEntity Certifications { get; set; }
    }
}
