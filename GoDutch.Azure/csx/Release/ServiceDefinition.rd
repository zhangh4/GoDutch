<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="GoDutch.Azure" generation="1" functional="0" release="0" Id="d274f78e-43d1-45a6-b5b4-7fbe05a80d06" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="GoDutch.AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="GoDutch:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/LB:GoDutch:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="GoDutch:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/MapGoDutch:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="GoDutchInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/MapGoDutchInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:GoDutch:Endpoint1">
          <toPorts>
            <inPortMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutch/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapGoDutch:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutch/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapGoDutchInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutchInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="GoDutch" generation="1" functional="0" release="0" software="C:\Users\khu9323\Documents\Visual Studio 2013\Projects\GoDutch\GoDutch.Azure\csx\Release\roles\GoDutch" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;GoDutch&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;GoDutch&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutchInstances" />
            <sCSPolicyUpdateDomainMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutchUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutchFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="GoDutchUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="GoDutchFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="GoDutchInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="6bb75e60-2115-45e6-bcae-8c8ed9732c7a" ref="Microsoft.RedDog.Contract\ServiceContract\GoDutch.AzureContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="c364d522-180f-4e58-ac0e-aa160ba3a975" ref="Microsoft.RedDog.Contract\Interface\GoDutch:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/GoDutch.Azure/GoDutch.AzureGroup/GoDutch:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>