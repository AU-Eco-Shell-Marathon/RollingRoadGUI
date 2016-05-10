Feature: Load and display saved data
	As a user, I want to load multiple
	previously collected datasets from files
	so that I can compare them.

	Scenario: Load data from single file
		Given a CSV-File in the DF4RR format
		When when I press load
		Then a window appears where I can select the file to open
			And a list of loaded files will be viewable
			
	Scenario: Select & display datasets
		Given that one or more dataset(s) has been loaded
		When I select a dataset
		Then a graph will be viewable
			And combined with other selected datasets