<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://wixtoolset.org/schemas/v4/wxs"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    version="1.0"
    exclude-result-prefixes="xsl wix">

	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

	<xsl:strip-space elements="*" />
	
	<!-- By default, copy all elements and nodes into the output... -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>	
	
	<xsl:key name="nlog.config" match="wix:Component[contains(wix:File/@Source, '$(var.ArtifactsPathWinInt)\nlog.config')]" use="@Id" />
	<xsl:template match="wix:Component[key('nlog.config', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('nlog.config', @Id)]" />

</xsl:stylesheet>
