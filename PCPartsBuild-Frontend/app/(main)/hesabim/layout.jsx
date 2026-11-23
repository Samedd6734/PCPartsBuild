'use client';

import React, { useState, useEffect } from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
// Not: Eğer avatar çekerken vs. kullanıcı verisi lazımsa buraya api ekleyebilirsin ama sadece iskelet olarak bu yeterli.

export default function AccountLayout({ children }) {
    const pathname = usePathname();

    return (
        <main className="w-full min-h-screen bg-[#0d1117] pt-12 pb-24 px-4 sm:px-8 lg:px-16 xl:px-24">
            {/* EN DIŞ GENİŞ KUTU (Tam Ekran Yayılım) */}
            <div className="w-full">

                {/* BAŞLIK KISMI */}
                <div className="mb-10 flex flex-col md:flex-row md:items-center justify-between border-b border-white/10 pb-6">
                    <div>
                        <h1 className="text-3xl font-bold text-white tracking-tight">Hesap Ayarları</h1>
                        <p className="text-white/60 mt-2 text-sm">Profil bilgilerinizi ve kayıtlı sistemlerinizi yönetin.</p>
                    </div>
                    <button onClick={() => { localStorage.clear(); window.location.href = '/'; }} className="mt-4 md:mt-0 px-6 py-2.5 bg-red-500/10 text-red-500 hover:bg-red-500/20 border border-red-500/20 rounded-lg font-bold text-sm transition-all flex items-center gap-2">
                        <span className="material-symbols-outlined text-lg">logout</span>
                        Çıkış Yap
                    </button>
                </div>

                {/* YAN YANA DURAN İKİ ANA SÜTUN (MENÜ VE İÇERİK) */}
                <div className="flex flex-col lg:flex-row items-start gap-12 w-full">

                    {/* SOL MENÜ (KAYA GİBİ SABİT, ASLA DARALMAZ: w-[300px]) */}
                    <aside className="w-full lg:w-[300px] flex-shrink-0 sticky top-28">

                        <div className="bg-[#111c22]/80 border border-white/5 rounded-2xl p-6 flex flex-col items-center text-center shadow-xl mb-6">
                            <div className="w-24 h-24 bg-gradient-to-br from-[#16a3b2] to-cyan-700 rounded-full flex items-center justify-center shadow-lg shadow-cyan-900/50 mb-4">
                                <span className="text-3xl font-black text-white">AH</span>
                            </div>
                            {/* İsim uzun olsa bile alt satıra geçmez, üç nokta koyar (truncate) */}
                            <h2 className="text-xl font-bold text-white w-full truncate">Abdussamed Hırabaşoğlu</h2>
                            <div className="mt-3 bg-green-500/10 border border-green-500/20 text-green-400 px-3 py-1 rounded-full text-xs font-bold uppercase tracking-widest">
                                Aktif Üye
                            </div>
                        </div>

                        <nav className="bg-[#111c22]/80 border border-white/5 rounded-2xl p-2 shadow-xl flex flex-col gap-1">
                            <Link href="/hesabim" className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-bold transition-all ${pathname === '/hesabim' ? 'bg-[#16a3b2]/20 text-[#16a3b2]' : 'text-white/60 hover:bg-white/5 hover:text-white'}`}>
                                <span className="material-symbols-outlined">person</span> Profil Bilgileri
                            </Link>
                            <Link href="/hesabim/sistemlerim" className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-bold transition-all ${pathname === '/hesabim/sistemlerim' ? 'bg-[#16a3b2]/20 text-[#16a3b2]' : 'text-white/60 hover:bg-white/5 hover:text-white'}`}>
                                <span className="material-symbols-outlined">computer</span> Kayıtlı Sistemlerim
                            </Link>
                            <Link href="/hesabim/favoriler" className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-bold transition-all ${pathname === '/hesabim/favoriler' ? 'bg-[#16a3b2]/20 text-[#16a3b2]' : 'text-white/60 hover:bg-white/5 hover:text-white'}`}>
                                <span className="material-symbols-outlined">favorite</span> Favoriler
                            </Link>
                            <Link href="/hesabim/ayarlar" className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-bold transition-all ${pathname === '/hesabim/ayarlar' ? 'bg-[#16a3b2]/20 text-[#16a3b2]' : 'text-white/60 hover:bg-white/5 hover:text-white'}`}>
                                <span className="material-symbols-outlined">lock</span> Şifre Değiştir
                            </Link>
                        </nav>
                    </aside>

                    {/* SAĞ İÇERİK (TÜM BOŞLUĞU DOLDURUR: flex-1) */}
                    <div className="flex-1 w-full min-w-0">
                        {children}
                    </div>

                </div>
            </div>
        </main>
    );
}