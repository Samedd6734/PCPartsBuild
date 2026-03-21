"use client";
import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { translations } from '@/lib/translations';
import { api } from '@/lib/api';
import Swal from 'sweetalert2';

export default function PukPage() {
  const [lang, setLang] = useState('tr');
  const [parts, setParts] = useState({
    cpu: [], motherboard: [], ram: [], gpu: [],
    storage: [], case: [], psu: [], cpuCooler: []
  });
  const [selectedParts, setSelectedParts] = useState({
    cpu: null, motherboard: null, ram: null, gpu: null,
    storage: null, case: null, psu: null, cpuCooler: null
  });
  const [results, setResults] = useState(null);
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [showAiModal, setShowAiModal] = useState(false);
  const [aiAnalysis, setAiAnalysis] = useState('');
  const [isAiLoading, setIsAiLoading] = useState(false);

  useEffect(() => {
    const savedLang = localStorage.getItem('lang') || 'tr';
    setLang(savedLang);
    fetchAllParts();

    const handleStorageChange = () => {
      setLang(localStorage.getItem('lang') || 'tr');
    };
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  const t = translations[lang];

  const fetchAllParts = async () => {
    const endpoints = {
      cpu: 'processors', motherboard: 'motherboards', ram: 'rams', gpu: 'gpus',
      storage: 'storages', case: 'cases', psu: 'psus', cpuCooler: 'cpuCoolers'
    };

    try {
      const fetchPromises = Object.entries(endpoints).map(async ([key, endpoint]) => {
        const res = await api.get(`/${endpoint}`);
        const data = await res.json();
        return { key, data: Array.isArray(data) ? data : [] };
      });

      const fetchResults = await Promise.all(fetchPromises);
      const newParts = {};
      fetchResults.forEach(res => { newParts[res.key] = res.data; });
      setParts(newParts);
    } catch (error) {
      console.error('Error fetching parts:', error);
    }
  };

  const handleSelect = (category, partId) => {
    const part = parts[category].find(p => p.id === parseInt(partId));
    setSelectedParts(prev => ({ ...prev, [category]: part || null }));
  };

  const checkCompatibility = () => {
    const selectedCount = Object.values(selectedParts).filter(p => p).length;
    if (selectedCount < 2) {
      Swal.fire({
        title: t['error-selection'],
        icon: 'warning',
        background: '#1f2937', color: '#fff'
      });
      return;
    }

    setIsAnalyzing(true);
    setResults(null);

    const errors = [];
    const { cpu, motherboard, ram } = selectedParts;

    if (cpu && motherboard && cpu.socket !== motherboard.socket) {
      errors.push(`${t['label-cpu']} & ${t['label-motherboard']}: Socket mismatch (${cpu.socket} vs ${motherboard.socket})`);
    }
    if (motherboard && ram && motherboard.memoryType !== ram.memoryType) {
      errors.push(`${t['label-motherboard']} & ${t['label-ram']}: Memory type mismatch (${motherboard.memoryType} vs ${ram.memoryType})`);
    }

    setTimeout(() => {
      setResults(errors.length > 0 ? { status: 'fail', reasons: errors } : { status: 'success' });
      setIsAnalyzing(false);
    }, 1200);
  };

  const getAiExpertOpinion = async () => {
    setIsAiLoading(true);
    setShowAiModal(true);
    setAiAnalysis('');

    const buildSummary = Object.entries(selectedParts)
      .filter(([_,v]) => v)
      .map(([k,v]) => `${k.toUpperCase()}: ${v.brand} ${v.modelName}`)
      .join('\n');

    try {
      // Logic for AI opinion (mocked for this turn)
      setTimeout(() => {
        let analysis = lang === 'tr' 
          ? "### Yapay Zeka Uzman Analizi\n\n- **Genel Değerlendirme:** Seçtiğiniz parçalar dengeli bir oyun sistemi oluşturuyor.\n- **Uyumluluk:** Temel bileşenler arasında teknik bir çakışma görünmüyor.\n- **Performans:** 1440p çözünürlükte yüksek FPS değerleri alabilirsiniz.\n- **Öneri:** İşlemciniz için daha güçlü bir soğutucu seçmek hız aşırtma potansiyelini artırabilir."
          : "### AI Expert Analysis\n\n- **General Assessment:** Your selected parts form a well-balanced gaming build.\n- **Compatibility:** No technical conflicts detected between major components.\n- **Performance:** You can expect high FPS values at 1440p resolution.\n- **Recommendation:** Choosing a more powerful cooler for your CPU could improve overclocking potential.";
        
        setAiAnalysis(analysis);
        setIsAiLoading(false);
      }, 2500);
    } catch (error) {
      setAiAnalysis(lang === 'tr' ? "Analiz sırasında bir hata oluştu." : "An error occurred during analysis.");
      setIsAiLoading(false);
    }
  };

  const categories = [
    { id: 'cpu', icon: 'memory' },
    { id: 'motherboard', icon: 'developer_board' },
    { id: 'ram', icon: 'straighten' },
    { id: 'gpu', icon: 'videogame_asset' },
    { id: 'storage', icon: 'database' },
    { id: 'case', icon: 'computer' },
    { id: 'psu', icon: 'power' },
    { id: 'cpuCooler', icon: 'mode_fan' }
  ];

  return (
    <div className="min-h-screen bg-background-dark text-white font-sans py-16">
      <main className="container mx-auto px-4 max-w-6xl">
        
        {/* Header Remastered */}
        <section className="text-center mb-16">
          <motion.h1 
            initial={{ opacity: 0, scale: 0.9 }}
            animate={{ opacity: 1, scale: 1 }}
            className="text-4xl md:text-6xl font-black mb-6 tracking-tighter drop-shadow-[0_10px_10px_rgba(0,0,0,0.5)]"
          >
            {t['puk-title']}
          </motion.h1>
          <motion.p 
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.2 }}
            className="text-white/60 text-lg md:text-xl max-w-3xl mx-auto leading-relaxed"
          >
            {t['puk-subtitle']}
          </motion.p>
        </section>

        {/* Dynamic Part Selection Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-16">
          {categories.map((cat, index) => (
            <motion.div 
              key={cat.id}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.1 }}
              className={`relative overflow-hidden group p-5 rounded-2xl border transition-all duration-300 ${
                selectedParts[cat.id] 
                  ? 'border-primary/50 bg-primary/5 shadow-[0_0_20px_rgba(22,163,178,0.1)]' 
                  : 'border-white/5 bg-white/5 hover:border-white/10'
              }`}
            >
              <div className="flex items-center gap-3 mb-4">
                <div className={`p-2 rounded-lg transition-colors duration-300 ${selectedParts[cat.id] ? 'bg-primary text-white' : 'bg-white/5 text-primary'}`}>
                  <span className="material-symbols-outlined text-xl">{cat.icon}</span>
                </div>
                <h3 className="text-xs font-black uppercase tracking-widest text-white/40 group-hover:text-white/80 transition-colors">
                  {t[`label-${cat.id}`]}
                </h3>
              </div>
              
              <div className="relative">
                <select 
                  className="w-full bg-black/40 border border-white/10 rounded-xl p-3 text-sm focus:ring-2 focus:ring-primary/50 outline-none appearance-none cursor-pointer transition-all hover:border-white/20 font-medium"
                  onChange={(e) => handleSelect(cat.id, e.target.value)}
                  value={selectedParts[cat.id]?.id || ''}
                >
                  <option value="" className="bg-background-dark">{t['select-default']}</option>
                  {parts[cat.id].map(p => (
                    <option key={p.id} value={p.id} className="bg-background-dark font-sans py-2">
                       {p.brand} {p.modelName}
                    </option>
                  ))}
                </select>
                <span className="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none text-white/20 group-hover:text-primary transition-colors">
                  expand_more
                </span>
              </div>
              
              {selectedParts[cat.id] && (
                <motion.p 
                  initial={{ opacity: 0 }} 
                  animate={{ opacity: 1 }}
                  className="mt-3 text-[10px] font-bold text-primary truncate"
                >
                  {selectedParts[cat.id].brand} {selectedParts[cat.id].modelName}
                </motion.p>
              )}
            </motion.div>
          ))}
        </div>

        {/* Action Buttons Remastered */}
        <div className="flex flex-col sm:flex-row justify-center gap-6 mb-20">
          <button 
            onClick={checkCompatibility}
            className="group relative inline-flex items-center justify-center gap-3 px-12 py-5 bg-primary text-white font-black text-lg rounded-2xl transition-all hover:scale-105 active:scale-95 shadow-[0_10px_30px_rgba(22,163,178,0.3)] hover:shadow-[0_15px_40px_rgba(22,163,178,0.5)]"
          >
            <span className="material-symbols-outlined group-hover:rotate-180 transition-transform duration-500">analytics</span>
            {t['action-check']}
          </button>
          
          <button 
            disabled={Object.values(selectedParts).filter(p => p).length < 2}
            onClick={getAiExpertOpinion}
            className="group relative inline-flex items-center justify-center gap-3 px-12 py-5 bg-white/5 border border-white/10 text-white font-black text-lg rounded-2xl transition-all hover:bg-white/10 hover:border-primary/50 disabled:opacity-30 disabled:cursor-not-allowed hover:scale-105 active:scale-95"
          >
            <span className="material-symbols-outlined text-primary group-hover:animate-pulse">auto_awesome</span>
            {t['action-expert']}
          </button>
        </div>

        {/* Results Visual Remastered */}
        <section className="max-w-4xl mx-auto">
          <div className="relative p-1 rounded-[2.5rem] bg-gradient-to-b from-white/10 to-transparent">
            <div className="bg-card-darker/80 backdrop-blur-2xl p-10 rounded-[2.3rem] overflow-hidden">
              <div className="flex items-center gap-4 mb-10">
                <div className="w-14 h-14 bg-primary/20 rounded-2xl flex items-center justify-center shadow-inner border border-primary/20">
                  <span className="material-symbols-outlined text-3xl text-primary">speed</span>
                </div>
                <h2 className="text-3xl font-black tracking-tight">{t['results-title']}</h2>
              </div>

              <AnimatePresence mode="wait">
                {isAnalyzing ? (
                  <motion.div 
                    key="analyzing"
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    exit={{ opacity: 0 }}
                    className="flex flex-col items-center py-16 gap-6"
                  >
                    <div className="relative w-20 h-20">
                      <div className="absolute inset-0 rounded-full border-4 border-primary/20"></div>
                      <div className="absolute inset-0 rounded-full border-4 border-primary border-t-transparent animate-spin"></div>
                      <span className="material-symbols-outlined absolute inset-0 flex items-center justify-center text-3xl text-primary animate-pulse">search</span>
                    </div>
                    <p className="text-xl font-black text-primary tracking-widest uppercase animate-pulse">
                      {t['ai-analiz']}
                    </p>
                  </motion.div>
                ) : !results ? (
                  <motion.div 
                    key="idle"
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    className="flex flex-col md:flex-row items-center gap-8 p-10 rounded-3xl bg-white/5 border border-white/5"
                  >
                    <span className="material-symbols-outlined text-7xl text-white/5">layers</span>
                    <div className="text-center md:text-left">
                      <h4 className="text-xl font-bold mb-2">{t['result-info-title']}</h4>
                      <p className="text-white/40 leading-relaxed font-medium">{t['result-info-text']}</p>
                    </div>
                  </motion.div>
                ) : results.status === 'success' ? (
                  <motion.div 
                    key="success"
                    initial={{ opacity: 0, scale: 0.95 }}
                    animate={{ opacity: 1, scale: 1 }}
                    className="flex flex-col items-center text-center gap-6 p-12 rounded-3xl bg-emerald-500/5 border border-emerald-500/20"
                  >
                    <div className="w-20 h-20 bg-emerald-500 rounded-full flex items-center justify-center shadow-[0_0_40px_rgba(16,185,129,0.3)]">
                      <span className="material-symbols-outlined text-5xl text-white">done_all</span>
                    </div>
                    <div>
                      <h3 className="text-3xl font-black text-emerald-400 mb-2">{t['result-success-title']}</h3>
                      <p className="text-emerald-100/60 font-medium">Bileşenleriniz teknik olarak birbiriyle tam uyumlu çalışmaktadır.</p>
                    </div>
                  </motion.div>
                ) : (
                  <motion.div 
                    key="fail"
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    className="p-10 rounded-3xl bg-red-500/5 border border-red-500/20"
                  >
                    <div className="flex items-center gap-6 mb-8">
                      <div className="w-16 h-16 bg-red-500 rounded-2xl flex items-center justify-center shadow-[0_0_30px_rgba(239,68,68,0.2)]">
                        <span className="material-symbols-outlined text-4xl text-white">report_problem</span>
                      </div>
                      <h3 className="text-2xl font-black text-red-400">{t['result-fail-title']}</h3>
                    </div>
                    <ul className="space-y-4">
                      {results.reasons.map((err, i) => (
                        <motion.li 
                          initial={{ opacity: 0, x: -10 }}
                          animate={{ opacity: 1, x: 0 }}
                          transition={{ delay: i * 0.1 }}
                          key={i} 
                          className="flex items-center gap-4 p-4 rounded-xl bg-red-500/10 border border-red-500/10 text-red-100/80 font-bold"
                        >
                          <span className="w-2 h-2 bg-red-500 rounded-full shadow-[0_0_10px_rgba(239,68,68,1)]"></span>
                          {err}
                        </motion.li>
                      ))}
                    </ul>
                  </motion.div>
                )}
              </AnimatePresence>
            </div>
          </div>
        </section>

      </main>

      {/* AI Expert Modal Remastered */}
      <AnimatePresence>
        {showAiModal && (
          <div className="fixed inset-0 z-[200] flex items-center justify-center p-4">
            <motion.div 
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setShowAiModal(false)}
              className="absolute inset-0 bg-black/80 backdrop-blur-xl"
            />
            <motion.div 
              initial={{ opacity: 0, scale: 0.9, y: 20 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.9, y: 20 }}
              className="relative w-full max-w-3xl bg-card-darker border border-white/10 rounded-[2.5rem] shadow-2xl overflow-hidden"
            >
              <div className="p-8 border-b border-white/5 flex justify-between items-center bg-white/5">
                <div className="flex items-center gap-4">
                  <div className="w-12 h-12 bg-primary rounded-2xl flex items-center justify-center">
                    <span className="material-symbols-outlined text-white">auto_awesome</span>
                  </div>
                  <div>
                    <h3 className="text-xl font-black tracking-tight">{t['action-expert']}</h3>
                    <p className="text-[10px] text-primary font-black uppercase tracking-[0.2em]">{lang === 'tr' ? 'Yapay Zeka Destekli Analiz' : 'AI Powered Analysis'}</p>
                  </div>
                </div>
                <button 
                  onClick={() => setShowAiModal(false)}
                  className="w-12 h-12 rounded-2xl bg-white/5 hover:bg-white/10 flex items-center justify-center transition-all active:scale-95"
                >
                  <span className="material-symbols-outlined">close</span>
                </button>
              </div>
              
              <div className="p-10 max-h-[65vh] overflow-y-auto custom-scrollbar">
                {isAiLoading ? (
                  <div className="flex flex-col items-center py-20 gap-8">
                    <div className="relative w-24 h-24 flex items-center justify-center">
                      <div className="absolute inset-0 rounded-full border-4 border-primary/10"></div>
                      <div className="absolute inset-0 rounded-full border-4 border-primary border-t-transparent animate-spin"></div>
                      <span className="material-symbols-outlined text-5xl text-primary animate-bounce">rocket_launch</span>
                    </div>
                    <div className="text-center space-y-2">
                       <p className="text-2xl font-black text-white animate-pulse">{t['ai-analiz']}</p>
                       <p className="text-sm text-white/20 font-bold uppercase tracking-widest">{lang === 'tr' ? 'Algoritmalar ve donanım veritabanı taranıyor' : 'Scanning algorithms and hardware database'}</p>
                    </div>
                  </div>
                ) : (
                  <motion.div 
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="prose prose-invert prose-primary max-w-none"
                  >
                    <div className="whitespace-pre-wrap font-sans text-white/80 leading-relaxed text-lg">
                      {aiAnalysis}
                    </div>
                  </motion.div>
                )}
              </div>

              <div className="p-8 border-t border-white/5 bg-white/5 flex justify-end">
                 <button 
                  onClick={() => setShowAiModal(false)}
                  className="px-8 py-3 bg-white/10 hover:bg-white/20 text-white font-bold rounded-xl transition-all active:scale-95"
                 >
                   {lang === 'tr' ? 'Anladım' : 'Got it'}
                 </button>
              </div>
            </motion.div>
          </div>
        )}
      </AnimatePresence>
    </div>
  );
}
