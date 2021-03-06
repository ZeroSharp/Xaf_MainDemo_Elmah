using System;
using System.Data.SqlClient;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.DatabaseUpdate {
	public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
           base.UpdateDatabaseAfterUpdateSchema();
           UpdateAnalysisCriteriaColumn();

           SecuritySystemRole defaultRole = CreateDefaultRole();

           Position developerPosition = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title == 'Developer'"));
           if (developerPosition == null) {
              developerPosition = ObjectSpace.CreateObject<Position>();
              developerPosition.Title = "Developer";
           }
           Position managerPosition = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title == 'Manager'"));
           if (managerPosition == null) {
              managerPosition = ObjectSpace.CreateObject<Position>();
              managerPosition.Title = "Manager";
           }

           Department devDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'Development Department'"));
           if (devDepartment == null) {
              devDepartment = ObjectSpace.CreateObject<Department>();
              devDepartment.Title = "Development Department";
              devDepartment.Office = "205";
              devDepartment.Positions.Add(developerPosition);
              devDepartment.Positions.Add(managerPosition);
           }

           Contact contactMary = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Mary' && LastName == 'Tellitson'"));
           if (contactMary == null) {
              contactMary = ObjectSpace.CreateObject<Contact>();
              contactMary.FirstName = "Mary";
              contactMary.LastName = "Tellitson";
              contactMary.Email = "mary_tellitson@md.com";
              contactMary.Birthday = new DateTime(1980, 11, 27);
              contactMary.Department = devDepartment;
              contactMary.Position = managerPosition;
           }

           Contact contactJohn = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'John' && LastName == 'Nilsen'"));
           if (contactJohn == null) {
              contactJohn = ObjectSpace.CreateObject<Contact>();
              contactJohn.FirstName = "John";
              contactJohn.LastName = "Nilsen";
              contactJohn.Email = "john_nilsen@md.com";
              contactJohn.Birthday = new DateTime(1981, 10, 3);
              contactJohn.Department = devDepartment;
              contactJohn.Position = developerPosition;
           }

           if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Review reports'")) == null) {
              DemoTask task = ObjectSpace.CreateObject<DemoTask>();
              task.Subject = "Review reports";
              task.AssignedTo = contactJohn;
              task.StartDate = DateTime.Parse("May 03, 2008");
              task.DueDate = DateTime.Parse("September 06, 2008");
              task.Status = DevExpress.Persistent.Base.General.TaskStatus.InProgress;
              task.Priority = Priority.High;
              task.EstimatedWork = 60;
              task.Description = "Analyse the reports and assign new tasks to employees.";
           }

           if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Fix breakfast'")) == null) {
              DemoTask task = ObjectSpace.CreateObject<DemoTask>();
              task.Subject = "Fix breakfast";
              task.AssignedTo = contactMary;
              task.StartDate = DateTime.Parse("May 03, 2008");
              task.DueDate = DateTime.Parse("May 04, 2008");
              task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
              task.Priority = Priority.Low;
              task.EstimatedWork = 1;
              task.ActualWork = 3;
              task.Description = "The Development Department - by 9 a.m.\r\nThe R&QA Department - by 10 a.m.";
           }
           if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Task1'")) == null) {
              DemoTask task = ObjectSpace.CreateObject<DemoTask>();
              task.Subject = "Task1";
              task.AssignedTo = contactJohn;
              task.StartDate = DateTime.Parse("June 03, 2008");
              task.DueDate = DateTime.Parse("June 06, 2008");
              task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
              task.Priority = Priority.High;
              task.EstimatedWork = 10;
              task.ActualWork = 15;
              task.Description = "A task designed specially to demonstrate the PivotChart module. Switch to the Reports navigation group to view the generated analysis.";
           }
           if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Task2'")) == null) {
              DemoTask task = ObjectSpace.CreateObject<DemoTask>();
              task.Subject = "Task2";
              task.AssignedTo = contactJohn;
              task.StartDate = DateTime.Parse("July 03, 2008");
              task.DueDate = DateTime.Parse("July 06, 2008");
              task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
              task.Priority = Priority.Low;
              task.EstimatedWork = 8;
              task.ActualWork = 16;
              task.Description = "A task designed specially to demonstrate the PivotChart module. Switch to the Reports navigation group to view the generated analysis.";
           }
           UpdateStatus("CreateAnalysis", "", "Creating analysis reports in the database...");
           CreateDataToBeAnalysed();
           UpdateStatus("CreateSecurityData", "", "Creating users and roles in the database...");
           #region Create a User for the Simple Security Strategy
           //// If a simple user named 'Sam' doesn't exist in the database, create this simple user
           //SecuritySimpleUser adminUser = ObjectSpace.FindObject<SecuritySimpleUser>(new BinaryOperator("UserName", "Sam"));
           //if(adminUser == null) {
           //    adminUser = ObjectSpace.CreateObject<SecuritySimpleUser>();
           //    adminUser.UserName = "Sam";
           //}
           //// Make the user an administrator
           //adminUser.IsAdministrator = true;
           //// Set a password if the standard authentication type is used
           //adminUser.SetPassword("");
           #endregion

           #region Create Users for the Complex Security Strategy
           // If a user named 'Sam' doesn't exist in the database, create this user
           SecuritySystemUser user1 = ObjectSpace.FindObject<SecuritySystemUser>(new BinaryOperator("UserName", "Sam"));
           if (user1 == null) {
              user1 = ObjectSpace.CreateObject<SecuritySystemUser>();
              user1.UserName = "Sam";
              // Set a password if the standard authentication type is used
              user1.SetPassword("");
           }
           // If a user named 'John' doesn't exist in the database, create this user
           SecuritySystemUser user2 = ObjectSpace.FindObject<SecuritySystemUser>(new BinaryOperator("UserName", "John"));
           if (user2 == null) {
              user2 = ObjectSpace.CreateObject<SecuritySystemUser>();
              user2.UserName = "John";
              // Set a password if the standard authentication type is used
              user2.SetPassword("");
           }
           // If a role with the Administrators name doesn't exist in the database, create this role
           SecuritySystemRole adminRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "Administrators"));
           if (adminRole == null) {
              adminRole = ObjectSpace.CreateObject<SecuritySystemRole>();
              adminRole.Name = "Administrators";
           }
           adminRole.IsAdministrative = true;

           // If a role with the Users name doesn't exist in the database, create this role
           SecuritySystemRole userRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "Users"));
           if (userRole == null) {
              userRole = ObjectSpace.CreateObject<SecuritySystemRole>();
              userRole.Name = "Users";
           }

           userRole.SetTypePermissionsRecursively<object>(SecurityOperations.FullAccess, SecuritySystemModifier.Allow);
           userRole.SetTypePermissionsRecursively<SecuritySystemUser>(SecurityOperations.FullAccess, SecuritySystemModifier.Deny);
           userRole.SetTypePermissionsRecursively<SecuritySystemRole>(SecurityOperations.FullAccess, SecuritySystemModifier.Deny);

            // Add the Administrators role to the user1
           user1.Roles.Add(adminRole);
           // Add the Users role to the user2
           user2.Roles.Add(userRole);
           user2.Roles.Add(defaultRole);
           #endregion

           ObjectSpace.CommitChanges();
        }
		private void CreateDataToBeAnalysed() {
			Analysis taskAnalysis1 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Completed tasks'"));
			if(taskAnalysis1 == null) {
				taskAnalysis1 = ObjectSpace.CreateObject<Analysis>();
				taskAnalysis1.Name = "Completed tasks";
				taskAnalysis1.ObjectTypeName = typeof(DemoTask).FullName;
				taskAnalysis1.Criteria = "[Status] = 'Completed'";
			}
			Analysis taskAnalysis2 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Estimated and actual work comparison'"));
			if(taskAnalysis2 == null) {
				taskAnalysis2 = ObjectSpace.CreateObject<Analysis>();
				taskAnalysis2.Name = "Estimated and actual work comparison";
				taskAnalysis2.ObjectTypeName = typeof(DemoTask).FullName;
			}
		}
		private void UpdateAnalysisCriteriaColumn() {
            if(((XPObjectSpace)ObjectSpace).Session.Connection is SqlConnection) {
				int length = (int)ExecuteScalarCommand(@"select CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.Columns WHERE TABLE_NAME = 'Analysis' AND COLUMN_NAME = 'Criteria'", true);
				if(length != -1) {
					ExecuteNonQueryCommand("alter table Analysis alter column Criteria nvarchar(max)", true);
				}
			}
		}
        private SecuritySystemRole CreateDefaultRole() {
            SecuritySystemRole defaultRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                defaultRole.Name = "Default";

                defaultRole.AddObjectAccessPermission<SecuritySystemUser>("[Oid] = CurrentUserId()", SecurityOperations.ReadOnlyAccess);
                defaultRole.AddMemberAccessPermission<SecuritySystemUser>("ChangePasswordOnFirstLogon", SecurityOperations.Write);
                defaultRole.AddMemberAccessPermission<SecuritySystemUser>("StoredPassword", SecurityOperations.Write);
                defaultRole.SetTypePermissionsRecursively<SecuritySystemRole>(SecurityOperations.Read, SecuritySystemModifier.Allow);
                defaultRole.SetTypePermissionsRecursively<AuditDataItemPersistent>(SecurityOperations.CRUDAccess, SecuritySystemModifier.Allow);
            }
            return defaultRole;
        }
    }
	public abstract class TaskAnalysis1LayoutUpdaterBase {
		protected abstract IAnalysisControl CreateAnalysisControl();
		protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
		public void Update(Analysis analysis) {
			if(analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis)) {
				IAnalysisControl control = CreateAnalysisControl();
				control.DataSource = new AnalysisDataSource(analysis, new XPCollection<DemoTask>(analysis.Session));
				control.Fields["Priority"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
				control.Fields["Subject"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
				control.Fields["AssignedTo.DisplayName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
				PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
			}
		}
	}
	public abstract class TaskAnalysis2LayoutUpdaterBase {
		protected abstract IAnalysisControl CreateAnalysisControl();
		protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
		public void Update(Analysis analysis) {
			if(analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis)) {
				IAnalysisControl control = CreateAnalysisControl();
				control.DataSource = new AnalysisDataSource(analysis, new XPCollection<DemoTask>(analysis.Session));
				control.Fields["Status"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
				control.Fields["Priority"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
				control.Fields["EstimatedWork"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
				control.Fields["ActualWork"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
				control.Fields["AssignedTo.DisplayName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
				PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
			}
		}
	}
}
