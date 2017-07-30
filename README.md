# PayslipCalculator
Just a test Web Api project

#Assumptions and implementation details:

- payslip period is always per calendar month so the start day is ignored if is not 1
- input payslip period is a date (02 March, 1/02/2017) , not an interval (1 March - 31 March)
- if the pay start date doesn't include the year, the current year is used
- future months are not accepted for payslip calculation
- super rate is provided as an integer between 0 and 50 inclusiv
- posting csv data requires a csv header containing the corresponding field names for the submitted data
- assuming tax rates and brackets can change in time they are loaded from the configuration file (application.json) but in the absence of configuration the default values are used

To run the project:
- Either create a site in IIS and map to the Web.Api project folder 
- Or  (using  IIS Express) from top menu select  > Debug > Start Debugging;

To test with csv format data
- Using Postman :
	- select method POST
	- enter the URL: http://<local-host-for-the-running-web-api>/payslip/calculatepayslips
	- under the Headers tab set the headers for csv: 
	      Content-Type:  text/csv 
		  Accept: text/csv  
	- under the Body tab select "raw" and format "Text" then enter the posted data in following format:
	        FirstName,LastName,AnnualSalary,SuperRate,PayStartPeriod
            Davi,Rudd,60050,9,01 March
            Ryan,Chen,120000,10,01 March 2017
	
Note: 
	


