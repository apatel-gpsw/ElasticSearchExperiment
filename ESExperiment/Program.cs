using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nest;

namespace ESExperiment
{
	class Program
	{
		public class Product
		{
			public string Title { get; set; }
			public string Brand { get; set; }
			public string Status { get; set; }
			public string SKU { get; set; }
		}

		static void Main(string[] args)
		{

			var nodeURI = new Uri("");

			var settings = new ConnectionSettings(
				nodeURI
			).DefaultFieldNameInferrer(p => p);

			var client = new ElasticClient(settings);

			//var person = new Person
			//{
			//	Id = "1",
			//	Firstname = "Martijn",
			//	Lastname = "Laarman"
			//};

			// var index = client.Index(person);

			//var searchResults = client.Search<Product>(p => p
			//	.From(0).Size(10)
			//		.Query(q => q
			//			.Bool(t => t
			//				.Must(u => u
			//					.Bool(v => v
			//						.Should(
			//							w => w.Match(x => x.Field("SKU").Query("1508017")),
			//							w => w.Match(x => x.Field("GTIN").Query("00886860404635"))
			//						)
			//					)
			//				)
			//			)
			//		)
			//	);
			Func<SearchDescriptor<Product>, ISearchRequest> query = p => p
				.From(0).Size(100)
					.Query(q => q
						.Bool(t => t
							.Must(u => u
								.Bool(v => v
									.Should(
										SkuBuilder("1508017", "1003319")
									)
								),
								w => w.Terms(z => z.Field(a => a.Status).Terms("active"))
							)
						)
					);

			SearchDescriptor<Product> sd = new SearchDescriptor<Product>()
				.From(0).Size(100)
					.Query(q => q
						.Bool(t => t
							.Must(u => u
								.Bool(v => v
									.Should(
										SkuBuilder("1508017", "1003319")
									)
								)
							)
						)
					);
			using(MemoryStream mStream = new MemoryStream())
			{
				client.Serializer.Serialize(sd, mStream);
				Console.WriteLine(Encoding.ASCII.GetString(mStream.ToArray()));
			}

			//Func<SearchDescriptor<Product>, ISearchRequest> q2 = (x) => x.
			var temo = client.Search<Product>(s => sd);

			var searchResults = client.Search(query);


			// Console.WriteLine(System.Text.Encoding.UTF8.GetString(searchResults.CallDetails.RequestBodyInBytes));

			//var searchResults = client.Search<Product>(s => s
			//	.Query(q => q
			//		.QueryString(qs => qs
			//			.Fields(f => f.Field(ff => ff.SKU))
			//			.Query("1508017")
			//		)
			//	)
			//);

			//searchResults = client.Search<Person>(p => p
			//	.From(0)
			//	.Size(10)
			//	.Query(q => q
			//	.Bool(r => r
			//	.Filter(s => s
			//	.Term(t => t.Name("Status").Value("active")))))
			//);
			foreach(var hit in searchResults.Hits)
			{
				Console.WriteLine(hit.Source.SKU);
			}
			Console.WriteLine("Total Hits: " + searchResults.Total);
		}

		private static IEnumerable<Func<QueryContainerDescriptor<Product>, QueryContainer>> SkuBuilder(params string[] skus)
		{
			yield return x => x.Match(y => y.Field("_legacy_full_text").Query("Mistletoe"));

			foreach(var sku in skus)
			{
				yield return x => x.Match(w => w.Field(ff => ff.SKU).Query(sku));
			}
		}

		private static Func<QueryContainerDescriptor<Product>, QueryContainer> test2()
		{
			//1508017
			return x => x.Match(w => w.Field(ff => ff.SKU).Query("1003319"));
		}
	}
}
