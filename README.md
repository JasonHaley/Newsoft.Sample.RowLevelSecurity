# Fork of Newsoft.Sample.RowLevelSecurity with a Web Project
A demo of row level security in entity framework 6 with an added Web Project to demo how to pull the tenantId from the claims on a user.

The Web project is just the plain New Web Project Template of a MVC App with authentication turned on.  

The HomeController adds 2 demo users with passwords:

jack@company.com
Password123!

jill@company.com
Password123!

jack has a tenant id claim added for tenant1
jill has a tenant id claim added for tenant2

The bulk of the changes I need to add to set the tenant id from the claim are in the TenantAwareDbContext.cs file.  I added a call to SetTenantIdFromClaim() in the Init method.

```
public void SetTenantIdFromClaim()
{
    var user = System.Threading.Thread.CurrentPrincipal;
    if (user.Identity.IsAuthenticated && user is ClaimsPrincipal)
    {
        var tenantIdClaim = ((ClaimsPrincipal)user).FindFirst(c => c.Type == "TenantId");
        if (tenantIdClaim != null)
        {
            var id = tenantIdClaim.Value;
            SetTenantId(Guid.Parse(id));
        }
    }
}
```

I also added an override for SaveChangesAsync().

NOTE:

You will need to modify the web.config connection string to point at your local database in order to get the demo working.  I have assumed you already had David Berube's sample up and running (starting with the Northwind backup file he provided).
