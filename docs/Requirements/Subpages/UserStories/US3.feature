Feature: Graph display
	As a user, I want to view graphs containing collected data 
	so that I quickly can create an overview.
	
	Scenario: View data in a graph
		Given some data has been collected
		When I select the "Live View" tab 
		Then I am able to see a graph with time as x-axis
		
	Scenario: Clear graph 
		Given some data has been collected
		When I press the "Clear" button
			And I don't want to save current data
		Then I am asked if I want to save data or not
			And the graph will be cleared