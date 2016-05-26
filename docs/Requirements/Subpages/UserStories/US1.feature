Feature: Collection of data
	As a user, I want to start and stop the collection of data
	so that I can control the flow of data.

	Scenario: Select data source (COM-Port)
		Given a program with no data source selected
		When I press "Select source"
			And select a COM-Port
			And press the select button
		Then a datasource will be selected and started
		
	Scenario: Start collection of data
		Given a data source is connected to the computer
			And the collection of data is stopped
		When I press the Start button
		Then the collection of data started
		
	Scenario: Stop collection of data
		Given a data source is connected to the computer
			And the collection of data is started
		When I press the Stop button
		Then the collection of data stopped