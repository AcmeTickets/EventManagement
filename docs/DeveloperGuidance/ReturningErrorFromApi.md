# How to return errors from an HTTP Web Service

HTTP Status Codes
When a client makes a request to an HTTP server — and the server successfully receives the request — the server must notify the client if the request was successfully handled or not.

HTTP accomplishes this with five categories of status codes:

100-level (Informational) – server acknowledges a request
200-level (Success) – server completed the request as expected
300-level (Redirection) – client needs to perform further actions to complete the request
400-level (Client error) – client sent an invalid request
500-level (Server error) – server failed to fulfill a valid request due to an error with server
Based on the response code, a client can summarise the result of a particular request.

As per Discussion we will return Generic 400 Error code from API instead of Business exception 
General Rules-

400 Bad Request-Return this from API in case of any Validation Failure or any Business Errors

   The 400 (Bad Request) status code indicates that the server cannot or
   will not process the request due to something that is perceived to be
   a client error (e.g., malformed request syntax, invalid request
   message framing, or deceptive request routing).
   Here we no need to send specific Business exception because of Owasp Standerds
   instead we send 400 (Bad Request)

   409 Conflict-

   The 409 (Conflict) status code indicates that the request could not
   be completed due to a conflict with the current state of the target
   resource.  This code is used in situations where the user might be
   able to resolve the conflict and resubmit the request.  The server
   SHOULD generate a payload that includes enough information for a user
   to recognize the source of the conflict.

   404 Not Found-

   The 404 (Not Found) status code indicates that the origin server did
   not find a current representation for the target resource or is not
   willing to disclose that one exists.  A 404 status code does not
   indicate whether this lack of representation is temporary or
   permanent; the 410 (Gone) status code is preferred over 404 if the
   origin server knows, presumably through some configurable means, that
   the condition is likely to be permanent.


 Based on Discussion following changes are needed in API to return 400 Error code
 Please note these changes are Temparory until new Validation layes is defined and developed

Example-

[HttpPost]
public async Task<IActionResult> Post([FromBody] StoreGarageLocation message)
{
if (!message.IsValid)
{
return this.BadRequest(message);
}

await this.session.Send(message);
return this.Ok(message);
}
 


      