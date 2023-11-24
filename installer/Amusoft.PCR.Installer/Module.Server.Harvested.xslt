<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://wixtoolset.org/schemas/v4/wxs"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    version="1.0"
    exclude-result-prefixes="xsl wix">

	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:strip-space elements="*" />
	
	<!-- Identity template: copies all other nodes as-is -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>

	<xsl:key name="appsettings.json" match="wix:Component[contains(wix:File/@Source, '$(var.ArtifactsPathWeb)\appsettings.json')]" use="@Id" />
	<xsl:template match="wix:Component[key('appsettings.json', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('appsettings.json', @Id)]" />

	<xsl:key name="appsettings.Development.json" match="wix:Component[contains(wix:File/@Source, '$(var.ArtifactsPathWeb)\appsettings.Development.json')]" use="@Id" />
	<xsl:template match="wix:Component[key('appsettings.Development.json', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('appsettings.Development.json', @Id)]" />
	
	<xsl:key name="nlog.config" match="wix:Component[contains(wix:File/@Source, '$(var.ArtifactsPathWeb)\nlog.config')]" use="@Id" />
	<xsl:template match="wix:Component[key('nlog.config', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('nlog.config', @Id)]" />
	
	<xsl:key name="Amusoft.PCR.App.Service.exe" match="wix:Component[contains(wix:File/@Source, '$(var.ArtifactsPathWeb)\Amusoft.PCR.App.Service.exe')]" use="@Id" />
	<xsl:template match="wix:Component[key('Amusoft.PCR.App.Service.exe', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('Amusoft.PCR.App.Service.exe', @Id)]" />

</xsl:stylesheet>