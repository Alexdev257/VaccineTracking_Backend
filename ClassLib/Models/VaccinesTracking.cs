﻿using System;
using System.Collections.Generic;

namespace ClassLib.Models;

public partial class VaccinesTracking
{
    public int Id { get; set; }

    public int VaccineId { get; set; }

    public int UserId { get; set; }

    public int ChildId { get; set; }

    public DateTime? MinimumIntervalDate { get; set; }

    public DateTime? VaccinationDate { get; set; }

    public DateTime? MaximumIntervalDate { get; set; }

    public int? PreviousVaccination { get; set; }

    public string Status { get; set; } = null!;

    public int AdministeredBy { get; set; }

    public string Reaction { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual Child Child { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Vaccine Vaccine { get; set; } = null!;
}
