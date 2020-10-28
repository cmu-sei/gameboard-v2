# Copyright 2020 Carnegie Mellon University. All Rights Reserved.
# Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

<#
.Synopsis Add a migration for multiple database providers
.Notes Assumes project is freshly built
#>
Param(
    [Parameter(Mandatory = $true)]
    $name
)

$providers = @('SqlServer', 'PostgreSQL')
foreach ($provider in $providers) {
    $env:Database:Provider=$provider
    dotnet ef migrations add $name --project ..\Gameboard.Data.$provider --no-build
}
$env:Database:Provider=''


