# Overview

This is sample demo of a two party Paystation integration using .NET Core

# Requirements

You will need a Paystation account setup to run this demo, this account will need to be 
approved for a [two party](https://docs.paystation.co.nz/#introduction-merchant-hosted-2-party) integration.

If you don't have a Paystation account or have an existing account without two party integrations allowed please [contact us](https://www2.paystation.co.nz/contact-us) and we'll help you out.

# Installation

Configuration of the application is done through the `appsettings.json` file read from the current working directory at startup.

You'll need to place your Paystation Id and Gateway Id in the corresponding keys in `appsettings.json`
