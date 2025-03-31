# EasyRestSharp

## Project Description
  This is an easy-to-use RESTful or Rest Web API client library. It's a wrapper of RestSharp which is very powerful. The goal of this library is to be very easy to use. Any suggestions or improvements to the project are welcome.

## How to use

### Post
  + Post json object
  ```csharp
  using var client = new EasyRestClient("https://api.example.com");
  var response = await client.PostAsync("/Person/Insert", new { name = "John", age = 30 });
  ```
  In none-generic version, it will throw error if there's an error in network/framework etc. you need to check the response status code and content by yourself. For example:
  ```csharp
  if (!response.IsSuccessful) {
     throw new Exception($"Status code: {response.StatusCode} Description:{response.StatusDescription}");
  }
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
  var multiPart = new RestMultiPart();
  multiPart.AddString("name", "John");
  multiPart.AddString("age", "30");
  multiPart.AddFile("file", filePath);
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
  In generic version, it will throw error if there's an error in network/framework or status code. you need to check the response status code and content by yourself.
### Get
  ```csharp
  var person = await client.GetAsync<Person>("/Person/FindByName", new { name = "John" });
  ```
### Other Methods
  Other methods like Put, Delete, Patch, Options are similar to Post and Get.
