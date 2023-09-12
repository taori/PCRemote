<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    version="1.0"
    exclude-result-prefixes="xsl wix">
	<!-- https://stackoverflow.com/questions/44765707/how-to-exclude-files-in-wix-toolset xsl is disgusting but rider is amazing -->

	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:strip-space elements="*" />

	<!--
    Find all <Component> elements with <File> elements with Source="" attributes ending in ".exe" and tag it with the "ExeToRemove" key.

    <Component Id="cmpSYYKP6B1M7WSD5KLEQ7PZW4YLOPYG61L" Directory="INSTALLDIR" Guid="*">
        <File Id="filKUS7ZRMJ0AOKDU6ATYY6IRUSR2ECPDFO" KeyPath="yes" Source="!(wix.StagingAreaPath)\ProofOfPEqualsNP.exe" />
    </Component>

    Because WiX's Heat.exe only supports XSLT 1.0 and not XSLT 2.0 we cannot use `ends-with( haystack, needle )` (e.g. `ends-with( wix:File/@Source, '.exe' )`...
    ...but we can use this longer `substring` expression instead (see https://github.com/wixtoolset/issues/issues/5609 )
    -->


	<!-- By default, copy all elements and nodes into the output... -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>


	<!-- rename root node -->
	<xsl:template match="wix:Wix">
		<Include><xsl:apply-templates select="@*|node()" /></Include>
	</xsl:template>

	<!-- patch relative paths -->
	<xsl:template match="wix:File/@Source">
		<xsl:attribute name="Source">
			<xsl:value-of select="concat(substring(.,0,19), '..\artifacts\msi\web\', substring(., 20))"/>
		</xsl:attribute>
	</xsl:template>
	
	<!-- remove by Id key -->

	<xsl:key
		name="ServerExe"
		match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 21 ) = 'Amusoft.PCR.Server.exe' ]"
		use="@Id" />
	<xsl:key
		name="SettingsToRemove"
		match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 15 ) = 'appsettings.json' ]"
		use="@Id" />
	<xsl:key
		name="DevSettingsToRemove"
		match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 27 ) = 'appsettings.Development.json' ]"
		use="@Id" />

	<xsl:key
		name="NLogToRemove"
		match="wix:Component[ substring( wix:File/@Source, string-length( wix:File/@Source ) - 10 ) = 'nlog.config' ]"
		use="@Id" />
	
	
	<xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'ServerExe', @Id ) ]" />
	
	<xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'SettingsToRemove', @Id ) ]" />
	<xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'DevSettingsToRemove', @Id ) ]" />

	<xsl:template match="*[ self::wix:Component or self::wix:ComponentRef ][ key( 'NLogToRemove', @Id ) ]" />

</xsl:stylesheet>