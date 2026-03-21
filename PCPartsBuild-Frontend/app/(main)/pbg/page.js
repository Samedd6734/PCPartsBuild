"use client";
import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { translations } from '@/lib/translations';

export default function PbgPage() {
  const [lang, setLang] = useState('tr');

  useEffect(() => {
    const savedLang = localStorage.getItem('lang') || 'tr';
    setLang(savedLang);

    const handleStorageChange = () => {
      setLang(localStorage.getItem('lang') || 'tr');
    };
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  const t = translations[lang];

  const components = [
    { id: 'cpu', icon: 'memory', image: '/Images/cpu.webp', delay: 0.1 },
    { id: 'gpu', icon: 'videogame_asset', image: '/Images/ekrankarti.webp', delay: 0.2 },
    { id: 'ram', icon: 'straighten', image: '/Images/ram.webp', delay: 0.3 },
    { id: 'ssd', icon: 'database', image: '/Images/ssd.webp', delay: 0.4 },
    { id: 'mobo', icon: 'developer_board', image: '/Images/anakart.webp', delay: 0.5 },
    { id: 'psu', icon: 'power', image: '/Images/guc.webp', delay: 0.6 },
    { id: 'case', icon: 'computer', image: '/Images/kasa.webp', delay: 0.7 },
    { id: 'cooler', icon: 'mode_fan', image: '/Images/sıvısogutma.webp', delay: 0.8 },
  ];

  const containerVariants = {
    hidden: { opacity: 0 },
    visible: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1
      }
    }
  };

  const itemVariants = {
    hidden: { opacity: 0, y: 30 },
    visible: { 
      opacity: 1, 
      y: 0,
      transition: { duration: 0.6, ease: "easeOut" }
    }
  };

  return (
    <div className="min-h-screen bg-background-dark text-white font-sans overflow-hidden">
      <main className="container mx-auto px-4 py-24 max-w-7xl">
        
        {/* Animated Header */}
        <section className="text-center mb-24 space-y-6">
          <motion.h1 
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            className="text-4xl md:text-7xl font-black tracking-tighter drop-shadow-2xl"
          >
            {t['pbg-title']}
          </motion.h1>
          <motion.div 
            initial={{ width: 0 }}
            animate={{ width: "100px" }}
            className="h-1 bg-primary mx-auto rounded-full"
          />
          <motion.p 
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ delay: 0.3 }}
            className="text-white/40 text-lg md:text-xl max-w-2xl mx-auto leading-relaxed font-medium"
          >
            {t['pbg-subtitle']}
          </motion.p>
        </section>

        {/* Components Grid */}
        <motion.div 
          variants={containerVariants}
          initial="hidden"
          animate="visible"
          className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8"
        >
          {components.map((comp) => (
            <motion.div 
              key={comp.id} 
              variants={itemVariants}
              whileHover={{ y: -10, scale: 1.02 }}
              className="group relative bg-white/5 backdrop-blur-md p-8 rounded-[2.5rem] border border-white/5 transition-all duration-300 hover:border-primary/40 hover:bg-white/10 shadow-2xl overflow-hidden"
            >
              {/* Abstract Background Element */}
              <div className="absolute -right-6 -top-6 w-24 h-24 bg-primary/20 blur-[50px] group-hover:bg-primary/30 transition-all"></div>
              
              <div className="relative z-10 flex flex-col h-full">
                <div className="mb-8 flex justify-between items-start">
                  <div className="w-14 h-14 bg-primary/20 rounded-2xl flex items-center justify-center text-primary group-hover:bg-primary group-hover:text-white transition-all duration-300 shadow-[inset_0_0_15px_rgba(22,163,178,0.2)]">
                    <span className="material-symbols-outlined text-3xl">{comp.icon}</span>
                  </div>
                  <span className="text-[10px] font-black tracking-[0.3em] text-white/10 group-hover:text-white/30 transition-colors uppercase">
                    Ref: 0{components.indexOf(comp) + 1}
                  </span>
                </div>

                <div className="h-48 flex items-center justify-center mb-10 p-6">
                  <motion.img 
                    whileHover={{ rotate: 5, scale: 1.1 }}
                    src={comp.image} 
                    alt={t[`${comp.id}-title`]} 
                    className="max-w-full max-h-full object-contain drop-shadow-[0_20px_30px_rgba(0,0,0,0.5)] transition-transform" 
                  />
                </div>

                <div className="space-y-4 mb-8">
                  <h3 className="text-2xl font-black tracking-tight group-hover:text-primary transition-colors">
                    {t[`${comp.id}-title`]}
                  </h3>
                  <p className="text-white/40 text-sm leading-relaxed font-medium line-clamp-3 group-hover:text-white/80 transition-colors">
                    {t[`${comp.id}-desc`]}
                  </p>
                </div>

                <div className="mt-auto pt-6 border-t border-white/5 flex items-center justify-between group-hover:border-primary/20 transition-colors">
                  <span className="text-[10px] font-black uppercase tracking-[0.2em] text-white/20 group-hover:text-primary transition-all">
                    {lang === 'tr' ? 'Detayları İncele' : 'View Details'}
                  </span>
                  <div className="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center group-hover:bg-primary/20 transition-all">
                    <span className="material-symbols-outlined text-sm text-white/20 group-hover:text-primary transition-all group-hover:translate-x-1">
                      arrow_forward
                    </span>
                  </div>
                </div>
              </div>
            </motion.div>
          ))}
        </motion.div>

        {/* Floating Background Effects */}
        <div className="fixed inset-0 pointer-events-none z-0">
          <div className="absolute top-1/4 -left-20 w-96 h-96 bg-primary/5 rounded-full blur-[120px]"></div>
          <div className="absolute bottom-1/4 -right-20 w-96 h-96 bg-indigo-500/5 rounded-full blur-[120px]"></div>
        </div>

      </main>
    </div>
  );
}
