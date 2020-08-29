# SPDiscover
View your stored procedures as HTML files which automatically link to related dependencies

# Introduction
This program is a tool I use at times to help clarify the relationships between database objects (particularly stored procedures) in SQL Server. The idea came about because I would be reading a stored procedure in one window which would call out to another stored procedure opened in another window which called a view which I had opened in another window etc. The problem I found was the SQL Managment Studio doesn't give you context in the tab to identify what is displayed on the tab unless you save the tab to a file, then you would have the file name. Regardless the navigation between related database objects was really non existent.

Microsoft does have the concept of database projects which does allow you to navigate database objects as if they where objects in a project, but not everyone wants or needs to create database projects simply to be able to make correlations between objects.


# Background
The program I have written is a standalone executable which when supplied with a starting procedure and a database connection creates an HTML file locally for the procedure requested as well as a file for each dependency. The program recurses through each dependency (tracks if a dependency has already been generated) and generates a section on the bottom with hyperlinks to open next. Being that we are now in the broswer you can use the browser navigation to move forward and back through all the objects.

Currently the program will generate a file for the following object types
- procedures
- tables
- views
- triggers
    
The program colorizes the generated HTML using Highlight.js with a Visual Studio 2015 dark mode

# Points of interest
The program illustrates a number of techniques including recursion, and using a T4 template to create the HTML file dynamically for each. It also contains the SQL which is used to get the dependencies and generate the SQL text including table definition.

# Using the program
In it's most basic form you would simply start the program with parameters. The program expects it's parameters to be key value pairs seperated with an equal (=) sign. The order of the parameters does not matter. Special parameters are as defined:

- procedure
  > procedure=dbo.CalculateTax
- outputtype
  > outputtype=txt (optional, normally HTML)
- launchwindow
  > launchwindow=true (optional, will open new window on navigation)
- help
- ShowConnectionString
  > for debugging purposes to see what connection string is being generated.
  
The program uses the SqlConnectionStringbuilder to create a valid string from parameters passed in. Each parameter is compared to the collection of keys which the builder is aware and generate a valid connection string. For example if the following were passed in:

	Data_Source=localhost;
	Initial_Catalog=AdventureWorks;
	Integrated_Security=True

would generate - Data Source=localhost;Initial Catalog=AdventureWorks;Integrated Security=True

# When procedures can be grouped by functionality
In my experience many large companies which depend on stored procedures to drive their business will group functionality together, some procedures may be shared across domains (maybe a proc that sends emails).

They may have a series of procedures which runs on a schedule to update information about customer updates and maybe another grouping of inventory.

You can create a folder for each type of grouping and add SPDiscover to your PATH variable. Doing this allows you to group files under the folder. To make it easier you may also want to create a batch file to supply consistent parameters in each folder.


