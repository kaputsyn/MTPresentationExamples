using System.Collections.Generic;

namespace Courier.Components.Authorize
{
    public interface AuthorizeActivityArguments
    {
        string CustomerName { get; }
        string CustomerCard { get; }
    }

    
}