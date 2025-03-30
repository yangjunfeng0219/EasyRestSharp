# EasyRestSharp
## Project Description
  This is an easy-to-use RESTful or Rest Web API client library. It's a wrapper of RestSharp which is very powerful. The goal of this library is to be very easy to use. Any suggestions or improvements to the project are welcome.
## How to use

### Post
  + Post json object
  ```csharp
  using var client = new Rest("https://api.example.com");
  var reponse = await client.PostAsync("/Person/Insert", new { name = "John", age = 30 });
  Rest.ThrowIfError(response);
  ```
  + if you want to pass query parameters, you can use ApplyQueryParameters/ApplySegmentParameters functions to generate the url.
  ```csharp
  var reponse = await client.PostAsync(
	RestUrl.ApplyQueryParameters("/Person/Insert", new {some properties}),
	new { name = "John", age = 30 }
	);
  ```
  + Post MultiPart
  ```csharp
  var multiPart = new MultiPartData();
  multiPart.Add("name", "John");
  multiPart.Add("age", 30);
  multiPart.Add("file", filePath);
  var reponse = await client.PostMultiPartAsync("/Person/Insert", multiPart);
  ```
  + Post pure string as body
  ```csharp
  var body = """
	{
		"name"="John",
		"age"=30
	}
  """;
  var reponse = await client.PostStringAsync("/Person/Insert", body, RestContentTypes.Json);
  ```
  + Generic Post
  ```csharp
  var count = await client.PostAsync<int>("/Person/Insert", new { name = "John", age = 30 });
  Console.WriteLine($"inserted count: {count}");
  ```
### Get
  ```csharp
  var person = await client.GetAsync<Person>("/Person/FindByName", new { name = "John" });
  ```
  