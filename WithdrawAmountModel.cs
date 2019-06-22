using System;
using System.ComponentModel.DataAnnotations;
public class WithdrawAmountModel    
{
    [Range(1, 200000000, ErrorMessage = "Invalid input.(1-200000000)")]
    public int withdrawAmount { get; set; }
}
