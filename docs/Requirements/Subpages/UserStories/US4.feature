Feature: Saving of data
	As a user, I want to save collected data 
	so that I am able to analyse it later.

	Scenario: Save to CSV-File
		Given some data has been collected
		When I press "Save"
		Then a window will open, where I can choose a file to save to.