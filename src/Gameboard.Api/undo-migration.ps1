# Copyright 2020 Carnegie Mellon University. All Rights Reserved.
# Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

<#
.Synopsis Remove last migration for multiple database providers
#>

$providers = @('SqlServer', 'PostgreSQL')
foreach ($provider in $providers) {
    $env:Database:Provider=$provider
    dotnet ef migrations remove --project ..\Gameboard.Data.$provider --no-build
}

$env:Database:Provider=''


