using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MTC.JMICS.Utility.Validators
{
    public sealed class ValidateLatitude : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value !=null && !string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                string latitudePattern = @"([0-9]{1,2})[:|°]([0-9]{1,2})[:|'|′]?([0-9]{1,2}(?:\.[0-9]+){0,1})?["" |""]([N | S])"; // Latitude DMS Pattern  

                bool isLatValid = Regex.IsMatch(value.ToString(), latitudePattern);
                if (!Regex.IsMatch(value.ToString(), latitudePattern))
                    return new ValidationResult("Please choose a valid country eg.(India,Pakistan,Nepal");
                else
                    return ValidationResult.Success;
            }
            return new ValidationResult("Latitude is Required");
        }

    }
}
