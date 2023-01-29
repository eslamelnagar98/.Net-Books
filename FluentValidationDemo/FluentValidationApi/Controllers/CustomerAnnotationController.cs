namespace FluentValidationApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerAnnotationController : ControllerBase
{
	private List<CustomerAnnotation> _annotations = new();

	[HttpPost]
	public ActionResult AddCustomerAnnotaion(CustomerAnnotation customerAnnotation)
	{
		_annotations.Add(customerAnnotation);
		return Ok();
	}
}
