using System.Collections.Generic;

namespace TradeReportETL.Pipeline.Modules.Transform.Models;

public record GleifApiResponse(List<LeiModel> Data);
public record LeiModel(string Id, AttributesModel Attributes);
public record AttributesModel(string Lei, EntityModel Entity, List<string> Bic);
public record EntityModel(LegalNameDto LegalName, LegalAddressDto LegalAddress);
public record LegalAddressDto(string Country);
public record LegalNameDto(string Name);
