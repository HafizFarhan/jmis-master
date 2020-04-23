
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MTC.JMICS.Models.DB
{
	[Table("tbl_Ships")]
	public partial class Ship
	{
		
		[Column("Ship_Id")]
		public virtual int ShipId { get; set; }
		[Key]
		[Column("IMO")]
		public virtual string IMO { get; set; }
		[Column("Ship_Name")]
		public virtual string ShipName { get; set; }
		[Column("Ex_Name")]
		public virtual string ExName { get; set; }
		[Column("Ship_Status")]
		public virtual string ShipStatus { get; set; }
		[Column("Flag_Name")]
		public virtual string FlagName { get; set; }
		[Column("Flag_Code")]
		public virtual string FlagCode { get; set; }
		[Column("MMSI")]
		public virtual string MMSI { get; set; }
		[Column("Call_Sign")]
		public virtual string CallSign { get; set; }
		[Column("Port_Of_Registry")]
		public virtual string PortOfRegistry { get; set; }
		[Column("Port_Of_Registry_Code")]
		public virtual string PortOfRegistryCode { get; set; }
		[Column("Date_Of_Build")]
		public virtual string DateOfBuild { get; set; }
		[Column("Gross_Tonnage")]
		public virtual int? GrossTonnage { get; set; }
		[Column("Dead_Weight")]
		public virtual int? DeadWeight { get; set; }
		[Column("Ship_Type_Level2")]
		public virtual string ShipTypeLevel2 { get; set; }
		[Column("Ship_Type_Level3")]
		public virtual string ShipTypeLevel3 { get; set; }
		[Column("Ship_Type_Level4")]
		public virtual string ShipTypeLevel4 { get; set; }
		[Column("Ship_Type_Level5")]
		public virtual string ShipTypeLevel5 { get; set; }
		[Column("Ship_Type_Level5_Hull_Type")]
		public virtual string ShipTypeLevel5HullType { get; set; }
		[Column("Ship_Type_Level5_Sub_Group")]
		public virtual string ShipTypeLevel5SubGroup { get; set; }
		[Column("Ship_Type_Level5_Sub_Type")]
		public virtual string ShipTypeLevel5SubType { get; set; }
		[Column("Ship_Type_Group")]
		public virtual string ShipTypeGroup { get; set; }
		[Column("Ship_Manager")]
		public virtual string ShipManager { get; set; }
		[Column("Ship_Manager_Company_Code")]
		public virtual string ShipManagerCompanyCode { get; set; }
		[Column("Registered_Owner")]
		public virtual string RegisteredOwner { get; set; }
		[Column("Registered_Owner_Code")]
		public virtual string RegisteredOwnerCode { get; set; }
		[Column("Group_Beneficial_Owner")]
		public virtual string GroupBeneficialOwner { get; set; }
		[Column("Group_Beneficial_Owner_Company_Code")]
		public virtual string GroupBeneficialOwnerCompanyCode { get; set; }
		[Column("Operator")]
		public virtual string Operator { get; set; }
		[Column("Operator_Company_Code")]
		public virtual string OperatorCompanyCode { get; set; }
		[Column("Photo_Present")]
		public virtual bool? PhotoPresent { get; set; } = false;
		[Column("SSMA_Time_Stamp")]
		public virtual byte[] SSMATimeStamp { get; set; }
		[Column("Created_On")]
		public virtual DateTime? CreatedOn { get; set; }
		[Column("Created_By")]
		public virtual string CreatedBy { get; set; }
		
		[Editable(false)]
		public virtual string PhotoContent { get; set; }

	}
}
