import { ChangeDetectionStrategy, Component, Input, SimpleChanges } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { EChartsCoreOption } from 'echarts/core';
import type { EChartsOption, SeriesOption } from 'echarts/types/dist/shared';
// import echarts core
import * as echarts from 'echarts/core';
// import necessary echarts components
import { BarChart } from 'echarts/charts';
import { LineChart } from 'echarts/charts';
import { GridComponent, TitleComponent, ToolboxComponent, TooltipComponent, LegendComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { CommonModule } from '@angular/common';
import { IEchartData } from '../../_models/echart-data';
import { Egraf } from '../../_models/egraf';
echarts.use([BarChart, LineChart, GridComponent, TitleComponent, ToolboxComponent, TooltipComponent, LegendComponent, CanvasRenderer]);

const seriesListEnergijaAPlus: SeriesOption[] = [];


@Component({
  selector: 'app-moj-elektro-graf',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, NgxEchartsDirective],
  templateUrl: './moj-elektro-graf.component.html',
  styleUrl: './moj-elektro-graf.component.css',
  providers: [
    provideEchartsCore({ echarts }),
  ]

})
export class MojElektroGrafComponent {
  // Input to receive selected merilno mesto data prirejeno za Echarts
  @Input() eChartsEnergijaAPlus: IEchartData | null = null;
  @Input() trimMode: 'none' | 'first4' | 'last2' = 'none';
  @Input() lineStyle: any = {
    width: 0.4,
    opacity: 0.7
  };
  @Input() legendConfig: any = {
    top: 40,
    type: 'scroll',
    selectedMode: true
  };
  chartOptions!: EChartsOption;



  ngOnChanges(changes: SimpleChanges) {
    const change = changes['eChartsEnergijaAPlus'];
    const labela = 'vse'; // hardcoded for now, later from input
    if (!change.isFirstChange() && this.eChartsEnergijaAPlus != null) {

      while (seriesListEnergijaAPlus.length) { seriesListEnergijaAPlus.pop(); }
      let index = 0;
      this.eChartsEnergijaAPlus.linesData.forEach(element => {

        if (this.eChartsEnergijaAPlus?.legendaOriginal[index] == labela || labela == "vse") {
          const legendName = this.eChartsEnergijaAPlus?.legend[index];
          const trimmedName = this.getTrimmedName(legendName);
          const seriesConfig: any = {
            type: 'line',
            lineStyle: this.lineStyle,
            smooth: true,
            data: element,
            name: trimmedName,
            //    animationDelay: (idx: number) => idx * 100,

            animationDelay: function (idx: number) { return idx * 20; },
            endLabel: {
              show: true,
              formatter: function (params: any) {
                return params.seriesName;
              }
            },
          };
          seriesListEnergijaAPlus.push(seriesConfig);
        }
        index++;
      });

      this.chartOptions = {
        title: {
          text: this.eChartsEnergijaAPlus.chartLabel,
          left: 'center'
        },
        legend: {
          ...this.legendConfig,
          data: this.eChartsEnergijaAPlus?.legend.map(item => this.getTrimmedName(item))
        },

        toolbox: {
          show: true,
          feature: {
            magicType: {
              type: ['bar', 'line', 'stack']
            },
            dataZoom: {
              yAxisIndex: 'none',
            },
          },
        },
        tooltip: {
          valueFormatter: function (value: any) {
            return value.toLocaleString('sl-SI') + ' kWh';
          },
          borderWidth: 1
        },

        xAxis: {
          data: this.eChartsEnergijaAPlus?.axisXLabels,
          silent: false,
          splitLine: {
            show: false,
          },
        },
        yAxis: {
          axisLabel: {
            formatter: function (value: any) {
              return value.toLocaleString('sl-SI') + ' kWh';
            },
          },
        },
        series: seriesListEnergijaAPlus,


        animationEasing: 'elasticOut',
        // animationDelayUpdate: (idx: number) => idx * 5,
        animationDelayUpdate: function (idx) {
          return idx * 30;
        }
      }


    }
  }

  private getTrimmedName(name: string | undefined): string | undefined {
    if (!name) return name;
    switch (this.trimMode) {
      case 'first4':
        return "Leto " + name.substring(0, 4);
      case 'last2':
        return "Blok " + name.substring(name.length - 1);
      case 'none':
      default:
        return name;
    }
  }
}

