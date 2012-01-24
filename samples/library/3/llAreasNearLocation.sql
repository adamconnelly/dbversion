CREATE FUNCTION [dbo].[llAreasNearLocation]
(
	@Latitude float,
	@Longitude float,
	@RadiusMetres int
)
RETURNS TABLE
AS
RETURN
(
	select *
	from
	(select geography::Point(@Latitude, @Longitude, 4326).STDistance(Location) "Distance",
		*
	from GeographicArea) dist
	where dist.Distance <= @RadiusMetres
)