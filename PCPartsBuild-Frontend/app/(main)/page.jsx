"use client";
import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { motion } from 'framer-motion';
import { translations } from '@/lib/translations';

export default function HomePage() {
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

    const t = translations[lang] || translations['tr'];

    const fadeInUp = {
        hidden: { opacity: 0, y: 20 },
        visible: { opacity: 1, y: 0, transition: { duration: 0.6, ease: "easeOut" } }
    };

    const staggerContainer = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: {
                staggerChildren: 0.2
            }
        }
    };

    return (
        <div className="relative flex flex-1 flex-col">
            {/* Arka Plan Videosu - 100% RAW QUALITY (No Filters) */}
            <video autoPlay id="bg-video" loop muted playsInline className="fixed inset-0 w-full h-full object-cover -z-10 pointer-events-none">
                <source src="/Videos/Video2.webm" type="video/webm" />
                Your browser does not support the video tag.
            </video>

            <main className="relative z-10 flex flex-1 flex-col w-full">
                {/* Hero Bölümü */}
                <motion.div 
                    initial="hidden"
                    animate="visible"
                    variants={staggerContainer}
                    className="flex flex-1 items-center justify-center py-24 text-center min-h-[60vh] drop-shadow-2xl"
                >
                    <div className="flex flex-col items-center gap-8 w-full px-4 md:px-8">
                        <motion.h1 
                            variants={fadeInUp}
                            className="text-white text-4xl font-black leading-tight tracking-[-0.033em] md:text-6xl lg:text-7xl max-w-5xl [text-shadow:_0_4px_16px_rgb(0_0_0_/_60%)]"
                        >
                            {t["hero-title"]}
                        </motion.h1>
                        <motion.h2 
                            variants={fadeInUp}
                            className="max-w-3xl text-white text-base font-bold leading-relaxed md:text-xl drop-shadow-lg"
                        >
                            {t["hero-subtitle"]}
                        </motion.h2>
                        <motion.div variants={fadeInUp}>
                            <Link href="/pts" className="flex min-w-[200px] cursor-pointer items-center justify-center overflow-hidden rounded-xl h-14 px-8 bg-primary text-white text-lg font-bold leading-normal tracking-[0.015em] transition-all hover:scale-105 hover:shadow-2xl hover:shadow-primary/30">
                                <span>{t["hero-button"]}</span>
                            </Link>
                        </motion.div>
                    </div>
                </motion.div>
                
                {/* Özellikler (Features) Bölümü - Containerized for padding */}
                <motion.div 
                    initial="hidden"
                    whileInView="visible"
                    viewport={{ once: true, margin: "-100px" }}
                    variants={staggerContainer}
                    className="flex flex-col gap-12 py-20 w-full px-6 sm:px-10 lg:px-16 xl:px-24"
                >
                    <div className="flex flex-col gap-4 text-center">
                        <motion.h1 
                            variants={fadeInUp}
                            className="text-white tracking-tight text-3xl font-bold leading-tight md:text-5xl md:font-black"
                        >
                            {t["features-main-title"]}
                        </motion.h1>
                        <motion.p 
                            variants={fadeInUp}
                            className="text-white/60 text-base font-normal leading-normal max-w-4xl mx-auto md:text-lg"
                        >
                            {t["features-main-subtitle"]}
                        </motion.p>
                    </div>
                    
                    <div className="grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-3 w-full pb-32">
                        {/* Özellik 1 - Elite Hover Animation */}
                        <motion.div 
                            variants={fadeInUp}
                            whileHover={{ scale: 1.03, y: -15 }}
                            className="group relative flex flex-1 flex-col gap-6 rounded-[2.5rem] border border-white/10 bg-[#111c22]/40 p-10 backdrop-blur-xl transition-all duration-500 hover:bg-[#111c22]/80 hover:border-primary/50 hover:shadow-[0_40px_80px_-20px_rgba(0,0,0,0.8),0_0_20px_rgba(22,163,178,0.1)] overflow-hidden"
                        >
                            {/* Animated Background Glow */}
                            <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-700 pointer-events-none" />
                            
                            <div className="relative z-10 text-primary bg-primary/10 w-16 h-16 rounded-2xl flex items-center justify-center transition-all duration-500 group-hover:bg-primary group-hover:text-white group-hover:shadow-[0_0_30px_rgba(22,163,178,0.6)] group-hover:-translate-y-1">
                                <span className="material-symbols-outlined text-4xl">verified</span>
                            </div>
                            <div className="relative z-10 flex flex-col gap-3">
                                <h2 className="text-white text-2xl font-black leading-tight group-hover:text-primary transition-colors tracking-tight">
                                    {t["feature-1-title"]}
                                </h2>
                                <p className="text-white/60 text-base font-medium leading-relaxed transition-colors group-hover:text-white/80">
                                    {t["feature-1-desc"]}
                                </p>
                            </div>
                        </motion.div>
                        
                        {/* Özellik 2 - Elite Hover Animation */}
                        <motion.div 
                            variants={fadeInUp}
                            whileHover={{ scale: 1.03, y: -15 }}
                            className="group relative flex flex-1 flex-col gap-6 rounded-[2.5rem] border border-white/10 bg-[#111c22]/40 p-10 backdrop-blur-xl transition-all duration-500 hover:bg-[#111c22]/80 hover:border-primary/50 hover:shadow-[0_40px_80px_-20px_rgba(0,0,0,0.8),0_0_20px_rgba(22,163,178,0.1)] overflow-hidden"
                        >
                            {/* Animated Background Glow */}
                            <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-700 pointer-events-none" />
                            
                            <div className="relative z-10 text-primary bg-primary/10 w-16 h-16 rounded-2xl flex items-center justify-center transition-all duration-500 group-hover:bg-primary group-hover:text-white group-hover:shadow-[0_0_30px_rgba(22,163,178,0.6)] group-hover:-translate-y-1">
                                <span className="material-symbols-outlined text-4xl">auto_fix_high</span>
                            </div>
                            <div className="relative z-10 flex flex-col gap-3">
                                <h2 className="text-white text-2xl font-black leading-tight group-hover:text-primary transition-colors tracking-tight">
                                    {t["feature-2-title"]}
                                </h2>
                                <p className="text-white/60 text-base font-medium leading-relaxed transition-colors group-hover:text-white/80">
                                    {t["feature-2-desc"]}
                                </p>
                            </div>
                        </motion.div>
                        
                        {/* Özellik 3 - Elite Hover Animation */}
                        <motion.div 
                            variants={fadeInUp}
                            whileHover={{ scale: 1.03, y: -15 }}
                            className="group relative flex flex-1 flex-col gap-6 rounded-[2.5rem] border border-white/10 bg-[#111c22]/40 p-10 backdrop-blur-xl transition-all duration-500 hover:bg-[#111c22]/80 hover:border-primary/50 hover:shadow-[0_40px_80px_-20px_rgba(0,0,0,0.8),0_0_20px_rgba(22,163,178,0.1)] overflow-hidden"
                        >
                            {/* Animated Background Glow */}
                            <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-700 pointer-events-none" />
                            
                            <div className="relative z-10 text-primary bg-primary/10 w-16 h-16 rounded-2xl flex items-center justify-center transition-all duration-500 group-hover:bg-primary group-hover:text-white group-hover:shadow-[0_0_30px_rgba(22,163,178,0.6)] group-hover:-translate-y-1">
                                <span className="material-symbols-outlined text-4xl">database</span>
                            </div>
                            <div className="relative z-10 flex flex-col gap-3">
                                <h2 className="text-white text-2xl font-black leading-tight group-hover:text-primary transition-colors tracking-tight">
                                    {t["feature-3-title"]}
                                </h2>
                                <p className="text-white/60 text-base font-medium leading-relaxed transition-colors group-hover:text-white/80">
                                    {t["feature-3-desc"]}
                                </p>
                            </div>
                        </motion.div>
                    </div>
                </motion.div>
            </main>
        </div>
    );
}
