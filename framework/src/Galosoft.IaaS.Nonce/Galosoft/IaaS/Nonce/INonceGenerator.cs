namespace Galosoft.IaaS.Nonce
{
    public interface INonceGenerator
    {
        string Generate(int size = 6);
    }
}
