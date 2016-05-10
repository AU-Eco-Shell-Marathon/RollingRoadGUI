Feature: Data readout
	As a user, I want to view graphs containing collected data
	so that I quickly can create an overview.

	Scenario: Read data
		Given some data has been collected
		When I select the "Live View" tab 
		Then I am able to read latest received data