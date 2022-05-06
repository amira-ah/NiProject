
      $(function() {
        var startdate,enddate;
        var formattedDate,formattedDate2;
        var DateFirst;
        var DateSecond;
        var seechecked;
    
     
 
           
         setValueFrom = function () {
         var datefrom = $("#date_from").data("kendoDatePicker").value();
         var dateto = $("#date_to").data("kendoDatePicker").value();
         if (dateto < datefrom) {
          $("#date_to").data("kendoDatePicker").value(datefrom);
         }
         $("#date_to").data("kendoDatePicker").min(datefrom);
           };
      
           $("#date_from").kendoDatePicker({
          change: setValueFrom
      
            });
      
           setValueTo = function () {
             calendar.value($("#date_to").val());
            };

          $("#date_to").kendoDatePicker({
              change: setValueTo
                  });

                  var element = document.getElementById("filterDate");

                  element.addEventListener('click', function() {
                    var datefrom = $("#date_from").data("kendoDatePicker").value();
                    var dateto = $("#date_to").data("kendoDatePicker").value();
                    var date_from=new Date(datefrom).toISOString();
                    var date_to=new Date(dateto).toISOString();
                    var dataSource = new kendo.data.DataSource({
                      transport: {
                        read: {
                          url: "https://localhost:44301/api/Fetch/get-Daily?getformat="+seechecked+"&getDatefrom="+date_from+"&getDateTo="+date_to,
                          dataType: "json"
                        }
                      },
                      pageSize: 10
                    });
                     
                  
                    $("#ordersGrid").kendoGrid({
                      dataSource: dataSource,
                      dataBound: function(e) {
                        var grid = e.sender,
                            chart = $("#ordersChart").data("kendoChart");
            
                        chart.dataSource.data(grid.dataSource.data());
                        grid.unbind("dataBound");
                      },
                      sortable: true,                  
                      schema: {
                        model: {
                          fields: {
                            DATETIME_KEY: { type: "date" },
                            NEALIAS: { type: "string" },
                            NETYPE: { type: "string" },
                            MAXRXLEVEL: { type: "double" },
                            MAXTXLEVEL: { type: "number" },
                            LINK: { type: "string" },
                            SUM_SLOT: { type: "double" },
                            RSLDEVIATION: { type: "double" },
                          }
                        }
                      },
                      pageable: true,
                    
                      columns: [{
                        field:"DATETIME_KEY",
                        title: "DATETIME KEY",
                        width: 200,
                        format: "{0: yyyy-MM-dd HH:mm:ss}"
                      
                      },{
                     
                        field: "NEALIAS",
                        title: "NEALIAS"
                      }
                    ,{
                     
                      field: "NETYPE",
                      title: "NETYPE"
                    },  {
                        field: "MAXRXLEVEL",
                        title: "MAX RX LEVEL",
                       
                      },
                      {
                        field: "MAXTXLEVEL",
                        title: "MAX TX LEVEL"
                      },{
                        field: "LINK",
                        title: "LINK",
                        width: 160,
                      
                      }, {
                        field: "SUM_SLOT",
                        title: "SLOT"
                      },{
                    
                      field: "RSLDEVIATION",
                      title: "RSLDEVIATION"
                    }]
                    });
                        $("#ordersChart").kendoChart({
                          dataSource: {
                              data: [],
                             sort: {
                             field: "DATETIME_KEY",
                               dir: "asc"
                               }
                             },
                          title: {
                            text: "KPI",
                            font: "20px sans-serif",
                            color: "#ff6800"
                          },
                          seriesDefaults: {
                          type: "line"
                          },
                          series: [{
                          field: "MAXRXLEVEL",
                          categoryField: "DATETIME_KEY",
                          }],
                          seriesClick: function(e) {
                          filterGrid(e.category);
                          },
                          axisLabelClick: function(e) {
                          filterGrid(e.value);
                          },
                          categoryAxis: {
                        
                          labels: {
                          rotation: -45,
                          visual: function(e) {
                          var visual = e.createVisual();
                          visual.options.cursor = "default";
                          return visual;
                          }
                          }
                          },
                          valueAxis: {
                            title: {
                            text: "Max RX Level"
                            },
                            labels: {
                            format: "{0:n0}"
                            }
                            },
                            tooltip: {
                            visible: true,
                            template: "#= category #: #= value # t"
                            }
                            });
                   }, false);
                   
                  var datefrom="";
                  var dateto="" ;
                  var gridDataSource;
                  $('input[type=radio][name=Hourly-Daily]').change(function() {
                    seechecked=this.value;
                    var datefrom = $("#date_from").val();
                    var dateto = $("#date_to").val();
                    if(datefrom!=""&& dateto!=""){
                      var date_from=new Date(datefrom).toISOString();
                      var date_to=new Date(dateto).toISOString();
                    }
                   
                    var dataSource = new kendo.data.DataSource({
                      transport: {
                        read: {
                          url: "https://localhost:44301/api/Fetch/get-Daily?getformat="+this.value+"&getDatefrom="+date_from+"&getDateTo="+date_to,
                          dataType: "json"
                        }
                      },
                      pageSize: 10
                    });
                     
                  
                    $("#ordersGrid").kendoGrid({
                      dataSource: dataSource,
                      dataBound: function(e) {
                        var grid = e.sender,
                            chart = $("#ordersChart").data("kendoChart");
            
                        chart.dataSource.data(grid.dataSource.data());
                        grid.unbind("dataBound");
                      },
                      sortable: true,                  
                      schema: {
                        model: {
                          fields: {
                            DATETIME_KEY: { type: "date" },
                            NEALIAS: { type: "string" },
                            NETYPE: { type: "string" },
                            MAXRXLEVEL: { type: "double" },
                            MAXTXLEVEL: { type: "number" },
                            LINK: { type: "string" },
                            SUM_SLOT: { type: "double" },
                            RSLDEVIATION: { type: "double" },
                          }
                        }
                      },
                      pageable: true,
                    
                      columns: [{
                        field:"DATETIME_KEY",
                        title: "DATETIME KEY",
                        width: 200,
                        template: "#= kendo.toString(DATETIME_KEY, 'dd/MMMM/yyyy H:mm:ss')  #"
                      
                      },{
                     
                        field: "NEALIAS",
                        title: "NEALIAS"
                      }
                    ,{
                     
                      field: "NETYPE",
                      title: "NETYPE"
                    },  {
                        field: "MAXRXLEVEL",
                        title: "MAX RX LEVEL",
                       
                      },
                      {
                        field: "MAXTXLEVEL",
                        title: "MAX TX LEVEL"
                      },{
                        field: "LINK",
                        title: "LINK",
                        width: 160,
                      
                      }, {
                        field: "SUM_SLOT",
                        title: "SLOT"
                      },{
                    
                      field: "RSLDEVIATION",
                      title: "RSLDEVIATION"
                    }]
                    });
                        $("#ordersChart").kendoChart({
                          dataSource: {
                              data: [],
                             sort: {
                             field: "DATETIME_KEY",
                               dir: "asc"
                               }
                             },
                          title: {
                            text: "KPI",
                            font: "20px sans-serif",
                            color: "#ff6800"
                          },
                          seriesDefaults: {
                          type: "line"
                          },
                          series: [{
                          field: "MAXRXLEVEL",
                          categoryField: "DATETIME_KEY",
                          }],
                          seriesClick: function(e) {
                          filterGrid(e.category);
                          },
                          axisLabelClick: function(e) {
                          filterGrid(e.value);
                          },
                          categoryAxis: {
                        
                          labels: {
                          rotation: -45,
                          visual: function(e) {
                          var visual = e.createVisual();
                          visual.options.cursor = "default";
                          return visual;
                          }
                          }
                          },
                          valueAxis: {
                            title: {
                            text: "Max RX Level"
                            },
                            labels: {
                            format: "{0:n0}"
                            }
                            },
                            tooltip: {
                            visible: true,
                            template: "#= category #: #= value # t"
                            }
                            });
                    
                     
                 

                  });
 
            
            /*      var dataSource = new kendo.data.DataSource({
                    transport: {
                      read: {
                        url: "https://localhost:44301/api/Fetch/get-Daily",
                        dataType: "json"
                      }
                    },
                    pageSize: 10
                  });
            
           $("#ordersGrid").kendoGrid({
                    dataSource: dataSource,
                    dataBound: function(e) {
                      var grid = e.sender,
                          chart = $("#ordersChart").data("kendoChart");
          
                      chart.dataSource.data(grid.dataSource.data());
                      grid.unbind("dataBound");
                    },
                    sortable: true,                  
                    schema: {
                      model: {
                        fields: {
                          DATETIME_KEY: { type: "date" },
                          NEALIAS: { type: "string" },
                          NETYPE: { type: "string" },
                          MAXRXLEVEL: { type: "double" },
                          MAXTXLEVEL: { type: "number" },
                          LINK: { type: "string" },
                          SUM_SLOT: { type: "double" },
                          RSLDEVIATION: { type: "double" },
                        }
                      }
                    },
                    pageable: true,
                  
                    columns: [{
                      field:"DATETIME_KEY",
                      title: "DATETIME KEY",
                      width: 200,
                      template: "#= kendo.toString(DATETIME_KEY, 'dd/MMMM/yyyy H:mm:ss')  #"
                    
                    },{
                   
                      field: "NEALIAS",
                      title: "NEALIAS"
                    }
                  ,{
                   
                    field: "NETYPE",
                    title: "NETYPE"
                  },  {
                      field: "MAXRXLEVEL",
                      title: "MAX RX LEVEL",
                     
                    },
                    {
                      field: "MAXTXLEVEL",
                      title: "MAX TX LEVEL"
                    },{
                      field: "LINK",
                      title: "LINK",
                      width: 160,
                    
                    }, {
                      field: "SUM_SLOT",
                      title: "SLOT"
                    },{
                  
                    field: "RSLDEVIATION",
                    title: "RSLDEVIATION"
                  }]
                  });

                  $("#ordersChart").kendoChart({
                    dataSource: {
                        data: [],
                       sort: {
                       field: "DATETIME_KEY",
                         dir: "asc"
                         }
                       },
                    title: {
                      text: "KPI",
                      font: "20px sans-serif",
                      color: "#ff6800"
                    },
                    seriesDefaults: {
                    type: "line"
                    },
                    series: [{
                    field: "MAXRXLEVEL",
                    categoryField: "DATETIME_KEY",
                    }],
                    seriesClick: function(e) {
                    filterGrid(e.category);
                    },
                    axisLabelClick: function(e) {
                    filterGrid(e.value);
                    },
                    categoryAxis: {
                  
                    labels: {
                    rotation: -45,
                    visual: function(e) {
                    var visual = e.createVisual();
                    visual.options.cursor = "default";
                    return visual;
                    }
                    }
                    },
                    valueAxis: {
                      title: {
                      text: "Max RX Level"
                      },
                      labels: {
                      format: "{0:n0}"
                      }
                      },
                      tooltip: {
                      visible: true,
                      template: "#= category #: #= value # t"
                      }
                      });
              */
      
/*

        $("#kendoVersion").text(kendo.version);

        var orderData = [
          { OrderID: 1, OrderDate: "2017-11-06T12:00:00", Freight: 12.34, ShipCity: "Antwerp", ShipCountry: "Belgium" },
          { OrderID: 2, OrderDate: "2019-03-02T12:00:00", Freight: 23.45, ShipCity: "Singapore", ShipCountry: "Singapore" },
          { OrderID: 3, OrderDate: "2019-06-26T12:00:00", Freight: 34.56, ShipCity: "Shanghai", ShipCountry: "China" },
          { OrderID: 4, OrderDate: "2017-09-20T12:00:00", Freight: 45.67, ShipCity: "Hamburg", ShipCountry: "Germany" },
          { OrderID: 5, OrderDate: "2017-12-12T12:00:00", Freight: 56.78, ShipCity: "Mumbai", ShipCountry: "India" },
          { OrderID: 6, OrderDate: "2018-02-08T12:00:00", Freight: 67.89, ShipCity: "Shanghai", ShipCountry: "China" },
          { OrderID: 7, OrderDate: "2018-05-05T12:00:00", Freight: 78.90, ShipCity: "Tokyo", ShipCountry: "Japan" },
          { OrderID: 8, OrderDate: "2019-08-03T12:00:00", Freight: 89.01, ShipCity: "Port Klang", ShipCountry: "Malaysia" },
          { OrderID: 9, OrderDate: "2018-10-29T12:00:00", Freight: 90.12, ShipCity: "Rotterdam", ShipCountry: "Netherlands" },
          { OrderID: 10, OrderDate: "2018-01-23T12:00:00", Freight: 10.32, ShipCity: "Vancouver", ShipCountry: "Canada" },
          { OrderID: 11, OrderDate: "2018-04-17T12:00:00", Freight: 21.43, ShipCity: "Colón", ShipCountry: "Panama" },
          { OrderID: 12, OrderDate: "2017-07-11T12:00:00", Freight: 32.54, ShipCity: "Manila", ShipCountry: "Philippines" },
          { OrderID: 13, OrderDate: "2017-10-24T12:00:00", Freight: 43.65, ShipCity: "Singapore", ShipCountry: "Singapore" },
          { OrderID: 14, OrderDate: "2018-03-11T12:00:00", Freight: 54.76, ShipCity: "Busan", ShipCountry: "South Korea" },
          { OrderID: 15, OrderDate: "2018-06-17T12:00:00", Freight: 65.87, ShipCity: "Shenzhen", ShipCountry: "China" },
          { OrderID: 16, OrderDate: "2018-10-13T12:00:00", Freight: 76.98, ShipCity: "Hong Kong", ShipCountry: "China" },
          { OrderID: 17, OrderDate: "2019-04-19T12:00:00", Freight: 87.09, ShipCity: "Dubai", ShipCountry: "UAE" },
          { OrderID: 18, OrderDate: "2019-07-25T12:00:00", Freight: 98.21, ShipCity: "Felixstowe", ShipCountry: "UK" },
          { OrderID: 19, OrderDate: "2017-09-22T12:00:00", Freight: 13.24, ShipCity: "Los Angeles", ShipCountry: "USA" },
          { OrderID: 20, OrderDate: "2018-12-09T12:00:00", Freight: 35.46, ShipCity: "New York", ShipCountry: "USA" },
          { OrderID: 21, OrderDate: "2018-02-04T12:00:00", Freight: 57.68, ShipCity: "Guangzhou", ShipCountry: "China" },
          { OrderID: 22, OrderDate: "2019-05-16T12:00:00", Freight: 9.87, ShipCity: "Long Beach", ShipCountry: "USA" },
          { OrderID: 23, OrderDate: "2019-08-18T12:00:00", Freight: 24.13, ShipCity: "Singapore", ShipCountry: "Singapore" }
        ];

        var gridDataSource = new kendo.data.DataSource({
          data: fulldata,
          schema: {
            model: {
              fields: {
                DATETIME_KEY: { type: "date" },
                LINK: { type: "string" },
                MAX_RX_LEVEL: { type: "double" },
                MAX_SLOT: { type: "double" },
                MAX_TX_LEVEL: { type: "number" },
                NEALIAS: { type: "string" },
                RSLDEVIATION: { type: "double" },
                SLOT: { type: "string" }
              }
            }
          },
          pageSize: 10,
          sort: {
            field: "DATETIME_KEY",
            dir: "desc"
          }
        });

     
        function filterGrid(country) {
          $("#ordersGrid").data("kendoGrid").dataSource.filter({
            field: "ShipCountry",
            operator: "eq",
            value: country
          });
        }

        $("#clearGridFilter").kendoButton({
          click: function(e) {
            $("#ordersGrid").data("kendoGrid").dataSource.filter({ });
          }
        });

        $("#ordersGrid").kendoGrid({
          dataSource: gridDataSource,
            
          height: 400,
          pageable: true,
          sortable: true,
          filterable: true,
         
          columns: [{
            field:"DATETIME_KEY",
            title: "DATETIME KEY",
            width: 200,
            format: "{0:MM/dd/yyyy}"
          }, {
            field: "LINK",
            title: "LINK",
            width: 160,
          
          },  {
            field: "MAX_RX_LEVEL",
            title: "MAX_RX_LEVEL",
          
          
          }, {
            field: "MAX_SLOT",
            title: "MAX_SLOT"
          },
        {
         
          field: "MAX_TX_LEVEL",
          title: "MAX_TX_LEVEL"
        },{
         
          field: "NEALIAS",
          title: "NEALIAS"
        },{
        
          field: "RSLDEVIATION",
          title: "RSLDEVIATION"
        },{
          
          field: "SLOT",
          title: "SLOT"
        }]
        });

*/    

   


 //   filterdate.addEventListener("click", function() {
   
 // });

});
function getFreightColor(freight) {
  if (freight > 60) {
    return "#090";
  } else if (freight < 30) {
    return "#f00";
  } else {
    return "#00c";
  }
}
