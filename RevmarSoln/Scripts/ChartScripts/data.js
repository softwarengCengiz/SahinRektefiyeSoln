﻿/*
 Highcharts JS v6.1.0 (2018-04-13)
 Data module

 (c) 2012-2017 Torstein Honsi

 License: www.highcharts.com/license
*/
(function (y) { "object" === typeof module && module.exports ? module.exports = y : y(Highcharts) })(function (y) {
	(function (h) {
	h.ajax = function (A) {
		var m = h.merge(!0, { url: !1, type: "GET", dataType: "json", success: !1, error: !1, data: !1, headers: {} }, A); A = { json: "application/json", xml: "application/xml", text: "text/plain", octet: "application/octet-stream" }; var r = new XMLHttpRequest; if (!m.url) return !1; r.open(m.type.toUpperCase(), m.url, !0); r.setRequestHeader("Content-Type", A[m.dataType] || A.text); h.objectEach(m.headers, function (h, m) {
			r.setRequestHeader(m,
				h)
		}); r.onreadystatechange = function () { var h; if (4 === r.readyState) { if (200 === r.status) { h = r.responseText; if ("json" === m.dataType) try { h = JSON.parse(h) } catch (F) { m.error && m.error(r, F); return } return m.success && m.success(h) } m.error && m.error(r, r.responseText) } }; try { m.data = JSON.stringify(m.data) } catch (v) { } r.send(m.data || !0)
	}
	})(y); (function (h) {
		var A = h.addEvent, m = h.Chart, r = h.win.document, v = h.each, y = h.objectEach, G = h.pick, D = h.inArray, E = h.isNumber, B = h.merge, H = h.splat, I = h.fireEvent, J = h.some, x, C = function (a, b, c) {
			this.init(a,
				b, c)
		}; h.extend(C.prototype, {
			init: function (a, b, c) {
				var f = a.decimalPoint, e; b && (this.chartOptions = b); c && (this.chart = c); "." !== f && "," !== f && (f = void 0); this.options = a; this.columns = a.columns || this.rowsToColumns(a.rows) || []; this.firstRowAsNames = G(a.firstRowAsNames, this.firstRowAsNames, !0); this.decimalRegex = f && new RegExp("^(-?[0-9]+)" + f + "([0-9]+)$"); this.rawColumns = []; this.columns.length && (this.dataFound(), e = !0); e || (e = this.fetchLiveData()); e || (e = !!this.parseCSV().length); e || (e = !!this.parseTable().length); e ||
					(e = this.parseGoogleSpreadsheet()); !e && a.afterComplete && a.afterComplete()
			}, getColumnDistribution: function () {
				var a = this.chartOptions, b = this.options, c = [], f = function (a) { return (h.seriesTypes[a || "line"].prototype.pointArrayMap || [0]).length }, e = a && a.chart && a.chart.type, d = [], k = [], p = 0, g; v(a && a.series || [], function (a) { d.push(f(a.type || e)) }); v(b && b.seriesMapping || [], function (a) { c.push(a.x || 0) }); 0 === c.length && c.push(0); v(b && b.seriesMapping || [], function (b) {
					var c = new x, n = d[p] || f(e), t = h.seriesTypes[((a && a.series ||
						[])[p] || {}).type || e || "line"].prototype.pointArrayMap || ["y"]; c.addColumnReader(b.x, "x"); y(b, function (a, b) { "x" !== b && c.addColumnReader(a, b) }); for (g = 0; g < n; g++)c.hasReader(t[g]) || c.addColumnReader(void 0, t[g]); k.push(c); p++
				}); b = h.seriesTypes[e || "line"].prototype.pointArrayMap; void 0 === b && (b = ["y"]); this.valueCount = { global: f(e), xColumns: c, individual: d, seriesBuilders: k, globalPointArrayMap: b }
			}, dataFound: function () {
				this.options.switchRowsAndColumns && (this.columns = this.rowsToColumns(this.columns)); this.getColumnDistribution();
				this.parseTypes(); !1 !== this.parsed() && this.complete()
			}, parseCSV: function (a) {
				function b(a, b, c, d) {
					function e(b) { l = a[b]; t = a[b - 1]; q = a[b + 1] } function f(a) { w.length < z + 1 && w.push([a]); w[z][w[z].length - 1] !== a && w[z].push(a) } function g() { h > u || u > K ? (++u, n = "") : (!isNaN(parseFloat(n)) && isFinite(n) ? (n = parseFloat(n), f("number")) : isNaN(Date.parse(n)) ? f("string") : (n = n.replace(/\//g, "-"), f("date")), p.length < z + 1 && p.push([]), c || (p[z][b] = n), n = "", ++z, ++u) } var k = 0, l = "", t = "", q = "", n = "", u = 0, z = 0; if (a.trim().length && "#" !== a.trim()[0]) {
						for (; k <
							a.length; k++) { e(k); if ("#" === l) { g(); return } if ('"' === l) for (e(++k); k < a.length && ('"' !== l || '"' === t || '"' === q);) { if ('"' !== l || '"' === l && '"' !== t) n += l; e(++k) } else d && d[l] ? d[l](l, n) && g() : l === m ? g() : n += l } g()
					}
				} function c(a) {
					var b = 0, c = 0, f = !1; J(a, function (a, d) {
						var e = !1, f, l, g = ""; if (13 < d) return !0; for (var k = 0; k < a.length; k++) {
							d = a[k]; f = a[k + 1]; l = a[k - 1]; if ("#" === d) break; else if ('"' === d) if (e) { if ('"' !== l && '"' !== f) { for (; " " === f && k < a.length;)f = a[++k]; "undefined" !== typeof u[f] && u[f]++; e = !1 } } else e = !0; else "undefined" !== typeof u[d] ?
								(g = g.trim(), isNaN(Date.parse(g)) ? !isNaN(g) && isFinite(g) || u[d]++ : u[d]++ , g = "") : g += d; "," === d && c++; "." === d && b++
						}
					}); f = u[";"] > u[","] ? ";" : ","; d.decimalPoint || (d.decimalPoint = b > c ? "." : ",", e.decimalRegex = new RegExp("^(-?[0-9]+)" + d.decimalPoint + "([0-9]+)$")); return f
				} function f(a, b) {
					var c, f, g = 0, k = !1, n = [], p = [], l; if (!b || b > a.length) b = a.length; for (; g < b; g++)if ("undefined" !== typeof a[g] && a[g] && a[g].length) for (c = a[g].trim().replace(/\//g, " ").replace(/\-/g, " ").split(" "), f = ["", "", ""], l = 0; l < c.length; l++)l < f.length &&
						(c[l] = parseInt(c[l], 10), c[l] && (p[l] = !p[l] || p[l] < c[l] ? c[l] : p[l], "undefined" !== typeof n[l] ? n[l] !== c[l] && (n[l] = !1) : n[l] = c[l], 31 < c[l] ? f[l] = 100 > c[l] ? "YY" : "YYYY" : 12 < c[l] && 31 >= c[l] ? (f[l] = "dd", k = !0) : f[l].length || (f[l] = "mm"))); if (k) { for (l = 0; l < n.length; l++)!1 !== n[l] ? 12 < p[l] && "YY" !== f[l] && "YYYY" !== f[l] && (f[l] = "YY") : 12 < p[l] && "mm" === f[l] && (f[l] = "dd"); 3 === f.length && "dd" === f[1] && "dd" === f[2] && (f[2] = "YY"); a = f.join("/"); return (d.dateFormats || e.dateFormats)[a] ? a : (I("deduceDateFailed"), "YYYY/mm/dd") } return "YYYY/mm/dd"
				}
				var e = this, d = a || this.options, k = d.csv, p; a = "undefined" !== typeof d.startRow && d.startRow ? d.startRow : 0; var g = d.endRow || Number.MAX_VALUE, h = "undefined" !== typeof d.startColumn && d.startColumn ? d.startColumn : 0, K = d.endColumn || Number.MAX_VALUE, m, t = 0, w = [], u = { ",": 0, ";": 0, "\t": 0 }; p = this.columns = []; k && d.beforeParse && (k = d.beforeParse.call(this, k)); if (k) {
					k = k.replace(/\r\n/g, "\n").replace(/\r/g, "\n").split(d.lineDelimiter || "\n"); if (!a || 0 > a) a = 0; if (!g || g >= k.length) g = k.length - 1; d.itemDelimiter ? m = d.itemDelimiter : (m = null,
						m = c(k)); for (var r = 0, t = a; t <= g; t++)"#" === k[t][0] ? r++ : b(k[t], t - a - r); d.columnTypes && 0 !== d.columnTypes.length || !w.length || !w[0].length || "date" !== w[0][1] || d.dateFormat || (d.dateFormat = f(p[0])); this.dataFound()
				} return p
			}, parseTable: function () {
				var a = this.options, b = a.table, c = this.columns, f = a.startRow || 0, e = a.endRow || Number.MAX_VALUE, d = a.startColumn || 0, k = a.endColumn || Number.MAX_VALUE; b && ("string" === typeof b && (b = r.getElementById(b)), v(b.getElementsByTagName("tr"), function (a, b) {
				b >= f && b <= e && v(a.children, function (a,
					e) { ("TD" === a.tagName || "TH" === a.tagName) && e >= d && e <= k && (c[e - d] || (c[e - d] = []), c[e - d][b - f] = a.innerHTML) })
				}), this.dataFound()); return c
			}, fetchLiveData: function () {
				function a(p) {
					function g(g, k, m) {
						function t() { e && b.liveDataURL === g && (b.liveDataTimeout = setTimeout(a, d)) } if (!g || 0 !== g.indexOf("http")) return g && c.error && c.error("Invalid URL"), !1; p && (clearTimeout(b.liveDataTimeout), b.liveDataURL = g); h.ajax({
							url: g, dataType: m || "json", success: function (a) { b && b.series && k(a); t() }, error: function (a, b) {
							3 > ++f && t(); return c.error &&
								c.error(b, a)
							}
						}); return !0
					} g(k.csvURL, function (a) { b.update({ data: { csv: a } }) }, "text") || g(k.rowsURL, function (a) { b.update({ data: { rows: a } }) }) || g(k.columnsURL, function (a) { b.update({ data: { columns: a } }) })
				} var b = this.chart, c = this.options, f = 0, e = c.enablePolling, d = 1E3 * (c.dataRefreshRate || 2), k = B(c); if (!c || !c.csvURL && !c.rowsURL && !c.columnsURL) return !1; 1E3 > d && (d = 1E3); delete c.csvURL; delete c.rowsURL; delete c.columnsURL; a(!0); return c && (c.csvURL || c.rowsURL || c.columnsURL)
			}, parseGoogleSpreadsheet: function () {
				function a(d) {
					var f =
						["https://spreadsheets.google.com/feeds/cells", c, e, "public/values?alt\x3djson"].join("/"); h.ajax({ url: f, dataType: "json", success: function (c) { d(c); b.enablePolling && setTimeout(function () { a(d) }, b.dataRefreshRate) }, error: function (a, c) { return b.error && b.error(c, a) } })
				} var b = this.options, c = b.googleSpreadsheetKey, f = this.chart, e = b.googleSpreadsheetWorksheet || 1, d = b.startRow || 0, k = b.endRow || Number.MAX_VALUE, p = b.startColumn || 0, g = b.endColumn || Number.MAX_VALUE, n = 1E3 * (b.dataRefreshRate || 2); 4E3 > n && (n = 4E3); c && (delete b.googleSpreadsheetKey,
					a(function (a) {
						var b = []; a = a.feed.entry; var c, e = (a || []).length, h = 0, n, m, q; if (!a || 0 === a.length) return !1; for (q = 0; q < e; q++)c = a[q], h = Math.max(h, c.gs$cell.col); for (q = 0; q < h; q++)q >= p && q <= g && (b[q - p] = []); for (q = 0; q < e; q++)c = a[q], h = c.gs$cell.row - 1, n = c.gs$cell.col - 1, n >= p && n <= g && h >= d && h <= k && (m = c.gs$cell || c.content, c = null, m.numericValue ? c = 0 <= m.$t.indexOf("/") || 0 <= m.$t.indexOf("-") ? m.$t : 0 < m.$t.indexOf("%") ? 100 * parseFloat(m.numericValue) : parseFloat(m.numericValue) : m.$t && m.$t.length && (c = m.$t), b[n - p][h - d] = c); v(b, function (a) {
							for (q =
								0; q < a.length; q++)void 0 === a[q] && (a[q] = null)
						}); f && f.series && f.update({ data: { columns: b } })
					})); return !1
			}, trim: function (a, b) { "string" === typeof a && (a = a.replace(/^\s+|\s+$/g, ""), b && /^[0-9\s]+$/.test(a) && (a = a.replace(/\s/g, "")), this.decimalRegex && (a = a.replace(this.decimalRegex, "$1.$2"))); return a }, parseTypes: function () { for (var a = this.columns, b = a.length; b--;)this.parseColumn(a[b], b) }, parseColumn: function (a, b) {
				var c = this.rawColumns, f = this.columns, e = a.length, d, k, h, g, m = this.firstRowAsNames, r = -1 !== D(b, this.valueCount.xColumns),
				v, t = [], w = this.chartOptions, u, x = (this.options.columnTypes || [])[b], w = r && (w && w.xAxis && "category" === H(w.xAxis)[0].type || "string" === x); for (c[b] || (c[b] = []); e--;)d = t[e] || a[e], h = this.trim(d), g = this.trim(d, !0), k = parseFloat(g), void 0 === c[b][e] && (c[b][e] = h), w || 0 === e && m ? a[e] = "" + h : +g === k ? (a[e] = k, 31536E6 < k && "float" !== x ? a.isDatetime = !0 : a.isNumeric = !0, void 0 !== a[e + 1] && (u = k > a[e + 1])) : (h && h.length && (v = this.parseDate(d)), r && E(v) && "float" !== x ? (t[e] = d, a[e] = v, a.isDatetime = !0, void 0 !== a[e + 1] && (d = v > a[e + 1], d !== u && void 0 !==
					u && (this.alternativeFormat ? (this.dateFormat = this.alternativeFormat, e = a.length, this.alternativeFormat = this.dateFormats[this.dateFormat].alternative) : a.unsorted = !0), u = d)) : (a[e] = "" === h ? null : h, 0 !== e && (a.isDatetime || a.isNumeric) && (a.mixed = !0))); r && a.mixed && (f[b] = c[b]); if (r && u && this.options.sort) for (b = 0; b < f.length; b++)f[b].reverse(), m && f[b].unshift(f[b].pop())
			}, dateFormats: {
				"YYYY/mm/dd": { regex: /^([0-9]{4})[\-\/\.]([0-9]{1,2})[\-\/\.]([0-9]{1,2})$/, parser: function (a) { return Date.UTC(+a[1], a[2] - 1, +a[3]) } },
				"dd/mm/YYYY": { regex: /^([0-9]{1,2})[\-\/\.]([0-9]{1,2})[\-\/\.]([0-9]{4})$/, parser: function (a) { return Date.UTC(+a[3], a[2] - 1, +a[1]) }, alternative: "mm/dd/YYYY" }, "mm/dd/YYYY": { regex: /^([0-9]{1,2})[\-\/\.]([0-9]{1,2})[\-\/\.]([0-9]{4})$/, parser: function (a) { return Date.UTC(+a[3], a[1] - 1, +a[2]) } }, "dd/mm/YY": { regex: /^([0-9]{1,2})[\-\/\.]([0-9]{1,2})[\-\/\.]([0-9]{2})$/, parser: function (a) { var b = +a[3], b = b > (new Date).getFullYear() - 2E3 ? b + 1900 : b + 2E3; return Date.UTC(b, a[2] - 1, +a[1]) }, alternative: "mm/dd/YY" }, "mm/dd/YY": {
					regex: /^([0-9]{1,2})[\-\/\.]([0-9]{1,2})[\-\/\.]([0-9]{2})$/,
					parser: function (a) { return Date.UTC(+a[3] + 2E3, a[1] - 1, +a[2]) }
				}
			}, parseDate: function (a) {
				var b = this.options.parseDate, c, f, e = this.options.dateFormat || this.dateFormat, d; if (b) c = b(a); else if ("string" === typeof a) {
					if (e) (b = this.dateFormats[e]) || (b = this.dateFormats["YYYY/mm/dd"]), (d = a.match(b.regex)) && (c = b.parser(d)); else for (f in this.dateFormats) if (b = this.dateFormats[f], d = a.match(b.regex)) { this.dateFormat = f; this.alternativeFormat = b.alternative; c = b.parser(d); break } d || (d = Date.parse(a), "object" === typeof d && null !==
						d && d.getTime ? c = d.getTime() - 6E4 * d.getTimezoneOffset() : E(d) && (c = d - 6E4 * (new Date(d)).getTimezoneOffset()))
				} return c
			}, rowsToColumns: function (a) { var b, c, f, e, d; if (a) for (d = [], c = a.length, b = 0; b < c; b++)for (e = a[b].length, f = 0; f < e; f++)d[f] || (d[f] = []), d[f][b] = a[b][f]; return d }, parsed: function () { if (this.options.parsed) return this.options.parsed.call(this, this.columns) }, getFreeIndexes: function (a, b) {
				var c, f = [], e = [], d; for (c = 0; c < a; c += 1)f.push(!0); for (a = 0; a < b.length; a += 1)for (d = b[a].getReferencedColumnIndexes(), c = 0; c <
					d.length; c += 1)f[d[c]] = !1; for (c = 0; c < f.length; c += 1)f[c] && e.push(c); return e
			}, complete: function () {
				var a = this.columns, b, c = this.options, f, e, d, k, h = [], g; if (c.complete || c.afterComplete) {
					if (this.firstRowAsNames) for (d = 0; d < a.length; d++)a[d].name = a[d].shift(); f = []; e = this.getFreeIndexes(a.length, this.valueCount.seriesBuilders); for (d = 0; d < this.valueCount.seriesBuilders.length; d++)g = this.valueCount.seriesBuilders[d], g.populateColumns(e) && h.push(g); for (; 0 < e.length;) {
						g = new x; g.addColumnReader(0, "x"); d = D(0, e); -1 !==
							d && e.splice(d, 1); for (d = 0; d < this.valueCount.global; d++)g.addColumnReader(void 0, this.valueCount.globalPointArrayMap[d]); g.populateColumns(e) && h.push(g)
					} 0 < h.length && 0 < h[0].readers.length && (g = a[h[0].readers[0].columnIndex], void 0 !== g && (g.isDatetime ? b = "datetime" : g.isNumeric || (b = "category"))); if ("category" === b) for (d = 0; d < h.length; d++)for (g = h[d], e = 0; e < g.readers.length; e++)"x" === g.readers[e].configName && (g.readers[e].configName = "name"); for (d = 0; d < h.length; d++) {
						g = h[d]; e = []; for (k = 0; k < a[0].length; k++)e[k] = g.read(a,
							k); f[d] = { data: e }; g.name && (f[d].name = g.name); "category" === b && (f[d].turboThreshold = 0)
					} a = { series: f }; b && (a.xAxis = { type: b }, "category" === b && (a.xAxis.uniqueNames = !1)); c.complete && c.complete(a); c.afterComplete && c.afterComplete(a)
				}
			}, update: function (a, b) { var c = this.chart; a && (a.afterComplete = function (a) { a.xAxis && c.xAxis[0] && a.xAxis.type === c.xAxis[0].options.type && delete a.xAxis; c.update(a, b, !0) }, B(!0, this.options, a), this.init(this.options)) }
		}); h.Data = C; h.data = function (a, b) { return new C(a, b) }; A(m, "init", function (a) {
			var b =
				this, c = a.args[0], f = a.args[1]; c && c.data && !b.hasDataDef && (b.hasDataDef = !0, b.data = new C(h.extend(c.data, { afterComplete: function (a) { var d, e; if (c.hasOwnProperty("series")) if ("object" === typeof c.series) for (d = Math.max(c.series.length, a && a.series ? a.series.length : 0); d--;)e = c.series[d] || {}, c.series[d] = B(e, a && a.series ? a.series[d] : {}); else delete c.series; c = B(a, c); b.init(c, f) } }), c, b), a.preventDefault())
		}); x = function () { this.readers = []; this.pointIsArray = !0 }; x.prototype.populateColumns = function (a) {
			var b = !0; v(this.readers,
				function (b) { void 0 === b.columnIndex && (b.columnIndex = a.shift()) }); v(this.readers, function (a) { void 0 === a.columnIndex && (b = !1) }); return b
		}; x.prototype.read = function (a, b) {
			var c = this.pointIsArray, f = c ? [] : {}, e; v(this.readers, function (d) { var e = a[d.columnIndex][b]; c ? f.push(e) : 0 < d.configName.indexOf(".") ? h.Point.prototype.setNestedProperty(f, e, d.configName) : f[d.configName] = e }); void 0 === this.name && 2 <= this.readers.length && (e = this.getReferencedColumnIndexes(), 2 <= e.length && (e.shift(), e.sort(), this.name = a[e.shift()].name));
			return f
		}; x.prototype.addColumnReader = function (a, b) { this.readers.push({ columnIndex: a, configName: b }); "x" !== b && "y" !== b && void 0 !== b && (this.pointIsArray = !1) }; x.prototype.getReferencedColumnIndexes = function () { var a, b = [], c; for (a = 0; a < this.readers.length; a += 1)c = this.readers[a], void 0 !== c.columnIndex && b.push(c.columnIndex); return b }; x.prototype.hasReader = function (a) { var b, c; for (b = 0; b < this.readers.length; b += 1)if (c = this.readers[b], c.configName === a) return !0 }
	})(y)
});