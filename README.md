# Assumptions and ambiguity 

### Variables 
- A book title is required for creation. Assumed to be passed via a post request as a string.

- A book publication date on creation should be null as it remains unpublished.

- ID is required for a book to be published, assumed to be passed via a post request as an int.

### Data Handling
- All data stored in memory for ease and simplicity.

- When listing books only title and publication status included. Other data omitted as not requested to include.

- No indentifer per book included in variables, assumed inclusion for ease of publication amid duplicates. Also assumed the book ID will be auto incremented rather than hardcoded.

### Structure 
- Typically I'd use an enum however the requirements stated to set status seemingly to strings so I followed the instructions.

- For the dates, no formatting was mentioned therefore no formatting features included and left simply as current date and timestamp.

### Testing
- Assumed a client required for WebApplicationFactory testing

- Business logic and validation should be tested

- Endpoints presumed accessible by application always