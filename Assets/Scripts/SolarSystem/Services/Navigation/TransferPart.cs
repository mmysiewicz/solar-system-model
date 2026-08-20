namespace SolarSystem
{
    public readonly struct TransferPart
    {
        public readonly Orbit From;
        public readonly Orbit To;
        public readonly ITransferManeuver Maneuver;

        public TransferPart(Orbit from, Orbit to, ITransferManeuver maneuver)
        {
            From = from;
            To = to;
            Maneuver = maneuver;
        }
    }
}