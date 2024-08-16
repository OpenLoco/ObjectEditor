namespace OpenLoco.Dat.Types.SCV5
{
	public enum LandDistributionPattern : uint8_t
	{
		Everywhere,
		Nowhere,
		FarFromWater,
		NearWater,
		OnMountains,
		FarFromMountains,
		InSmallRandomAreas,
		InLargeRandomAreas,
		AroundCliffs,
	}
}
