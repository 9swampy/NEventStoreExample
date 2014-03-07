namespace NEventStoreExample.Model
{
    // On to the concrete part of the spike... we have accounts, accounts are an aggregate in my domain.
    // For this spike, accounts have a name, are active or inactive (in the real world, they're deactivated for many reasons, but not here)
    // and an account has an address (in the real world, they actually have a couple addresses. Again, not germane to this spike)
    //  
    // 
    // This is just a value object in DDD parlance. It has no identity itself because it's always owned by an entity object.
    public class Address
    {
        public Address(string line1, string city, string state)
        {
            Line1 = line1;
            City = city;
            State = state;
        }

        public string Line1 { get; private set; }
        
        public string City { get; private set; }
        
        public string State { get; private set; }
    }
}